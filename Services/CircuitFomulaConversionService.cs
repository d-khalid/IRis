using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IRis.Services;
public class CircuitFormulaConversionService
{
    public class CircuitComponent
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<string> InputWires { get; set; } = new List<string>();
        public string OutputWire { get; set; }
        public string Value { get; set; } // For LogicToggle components
    }

    public class CircuitFormula
    {
        public string OutputName { get; set; }
        public string Formula { get; set; }
        public List<string> InputVariables { get; set; } = new List<string>();
    }

    public static List<CircuitFormula> ConvertXmlToFormulas(string xmlFilePath)
    {
        string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
        return ConvertXmlContentToFormulas(xmlContent);
    }

    public static List<CircuitFormula> ConvertXmlContentToFormulas(string xmlContent)
    {
        XDocument doc = XDocument.Parse(xmlContent);
        
        var components = new Dictionary<string, CircuitComponent>();
        var wireToOutputComponent = new Dictionary<string, string>();
        var wireToInputComponents = new Dictionary<string, List<string>>();
        var inputToggleNames = new Dictionary<string, string>();
        
        // Parse components
        foreach (var componentEl in doc.Descendants("Component"))
        {
            string type = componentEl.Attribute("Type")?.Value;
            string componentId = Guid.NewGuid().ToString(); // Generate unique ID for component
            
            var component = new CircuitComponent
            {
                Id = componentId,
                Type = type
            };
            
            // Get terminals and connected wires
            var terminals = componentEl.Descendants("Terminal").ToList();
            
            if (type == "LogicToggle")
            {
                // For input toggles, the wire is an output
                var wireIds = terminals.FirstOrDefault()?.Descendants("guid").Select(g => g.Value).ToList();
                if (wireIds?.Any() == true)
                {
                    component.OutputWire = wireIds.First();
                    wireToOutputComponent[wireIds.First()] = componentId;
                    
                    // Create a meaningful name for this input
                    string inputName = $"Input_{inputToggleNames.Count + 1}";
                    inputToggleNames[componentId] = inputName;
                }
            }
            else if (type == "LogicProbe")
            {
                // For output probes, the wire is an input
                var wireIds = terminals.FirstOrDefault()?.Descendants("guid").Select(g => g.Value).ToList();
                if (wireIds?.Any() == true)
                {
                    component.InputWires.Add(wireIds.First());
                    if (!wireToInputComponents.ContainsKey(wireIds.First()))
                        wireToInputComponents[wireIds.First()] = new List<string>();
                    wireToInputComponents[wireIds.First()].Add(componentId);
                }
            }
            else
            {
                // For logic gates, first terminals are inputs, last is output
                for (int i = 0; i < terminals.Count - 1; i++)
                {
                    var wireIds = terminals[i].Descendants("guid").Select(g => g.Value).ToList();
                    foreach (var wireId in wireIds)
                    {
                        component.InputWires.Add(wireId);
                        if (!wireToInputComponents.ContainsKey(wireId))
                            wireToInputComponents[wireId] = new List<string>();
                        wireToInputComponents[wireId].Add(componentId);
                    }
                }
                
                // Last terminal is output
                if (terminals.Count > 0)
                {
                    var outputWireIds = terminals.Last().Descendants("guid").Select(g => g.Value).ToList();
                    if (outputWireIds.Any())
                    {
                        component.OutputWire = outputWireIds.First();
                        wireToOutputComponent[outputWireIds.First()] = componentId;
                    }
                }
            }
            
            components[componentId] = component;
        }
        
        // Generate formulas for each output probe
        var formulas = new List<CircuitFormula>();
        var outputProbes = components.Values.Where(c => c.Type == "LogicProbe").ToList();
        
        for (int i = 0; i < outputProbes.Count; i++)
        {
            var probe = outputProbes[i];
            string outputName = $"Output_{i + 1}";
            
            if (i == 0 && outputProbes.Count == 2)
                outputName = "Sum"; // First output of full adder is typically sum
            else if (i == 1 && outputProbes.Count == 2)
                outputName = "Carry"; // Second output is typically carry
            
            // Add safety check here
            if (!probe.InputWires.Any())
            {
                Console.WriteLine($"Warning: Output probe {i + 1} has no input wires connected.");
                continue; // Skip this probe or provide a default formula
            }
            
            string formula = BuildFormula(probe.InputWires.First(), components, wireToOutputComponent, inputToggleNames);
            
            var circuitFormula = new CircuitFormula
            {
                OutputName = outputName,
                Formula = formula,
                InputVariables = ExtractInputVariables(formula)
            };
            
            formulas.Add(circuitFormula);
        }
        
        return formulas;
    }

    public static int GetNumberOfInputs(string xmlContent)
    {
        XDocument doc = XDocument.Parse(xmlContent);
        return doc.Descendants("Component")
                  .Count(c => c.Attribute("Type")?.Value == "LogicToggle");
    }

    public static int GetNumberOfInputsFromFile(string xmlFilePath)
    {
        string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
        return GetNumberOfInputs(xmlContent);
    }

    public static int GetNumberOfOutputs(string xmlContent)
    {
        XDocument doc = XDocument.Parse(xmlContent);
        return doc.Descendants("Component")
                  .Count(c => c.Attribute("Type")?.Value == "LogicProbe");
    }

    public static int GetNumberOfOutputsFromFile(string xmlFilePath)
    {
        string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
        return GetNumberOfOutputs(xmlContent);
    }
    
    private static string BuildFormula(string wireId, Dictionary<string, CircuitComponent> components, 
        Dictionary<string, string> wireToOutputComponent, Dictionary<string, string> inputToggleNames)
    {
        if (!wireToOutputComponent.ContainsKey(wireId))
            return "0"; // Wire not connected to any output
            
        string componentId = wireToOutputComponent[wireId];
        var component = components[componentId];
        
        switch (component.Type)
        {
            case "LogicToggle":
                return inputToggleNames[componentId];
                
            case "XorGate":
                var xorInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"({string.Join(" XOR ", xorInputs)})";
                
            case "AndGate":
                var andInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"({string.Join(" AND ", andInputs)})";
                
            case "OrGate":
                var orInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"({string.Join(" OR ", orInputs)})";

            case "NotGate":
                if (!component.InputWires.Any())
                    return "0";
                var notInput = BuildFormula(component.InputWires.First(), components, wireToOutputComponent, inputToggleNames);
                return $"(NOT {notInput})";

            case "NandGate":
                var nandInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"(NOT ({string.Join(" AND ", nandInputs)}))";

            case "NorGate":
                var norInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"(NOT ({string.Join(" OR ", norInputs)}))";

            case "XnorGate":
                var xnorInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"(NOT ({string.Join(" XOR ", xnorInputs)}))";
                
            default:
                return "Unknown";
        }
    }
    
    private static List<string> ExtractInputVariables(string formula)
    {
        var inputs = new HashSet<string>();
        var tokens = formula.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var token in tokens)
        {
            if (token.StartsWith("Input_"))
            {
                inputs.Add(token);
            }
        }
        
        return inputs.OrderBy(x => x).ToList();
    }
}