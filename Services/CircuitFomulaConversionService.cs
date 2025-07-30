using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace IRis.Services;
public class CircuitFormulaConversionService
{
    public class CircuitComponent
    {
        public required string Id { get; set; }
        public required string Type { get; set; }
        public List<string> InputWires { get; set; } = new List<string>();
        public string OutputWire { get; set; } = null!;
        public string Value { get; set; } = null!; // For LogicToggle components
    }

    public class CircuitFormula
    {
        public required string OutputName { get; set; }
        public required string Formula { get; set; }
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
            string type = componentEl.Attribute("Type")?.Value!;
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
                // XOR: A XOR B = (A & !B) | (!A & B)
                if (component.InputWires.Count == 2)
                {
                    var input1 = BuildFormula(component.InputWires[0], components, wireToOutputComponent, inputToggleNames);
                    var input2 = BuildFormula(component.InputWires[1], components, wireToOutputComponent, inputToggleNames);
                    return $"(({input1}&!{input2})|(!{input1}&{input2}))";
                }
                else
                {
                    // For multiple inputs, chain XOR operations
                    var inputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                    var result = inputs[0];
                    for (int i = 1; i < inputs.Count; i++)
                    {
                        result = $"(({result}&!{inputs[i]})|(!{result}&{inputs[i]}))";
                    }
                    return result;
                }
                
            case "AndGate":
                var andInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"({string.Join("&", andInputs)})";
                
            case "OrGate":
                var orInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"({string.Join("|", orInputs)})";

            case "NotGate":
                if (!component.InputWires.Any())
                    return "0";
                var notInput = BuildFormula(component.InputWires.First(), components, wireToOutputComponent, inputToggleNames);
                return $"(!{notInput})";

            case "NandGate":
                // NAND: !(A & B & C...)
                var nandInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"!({string.Join("&", nandInputs)})";

            case "NorGate":
                // NOR: !(A | B | C...)
                var norInputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                return $"!({string.Join("|", norInputs)})";

            case "XnorGate":
                // XNOR: !(A XOR B) = (A & B) | (!A & !B)
                if (component.InputWires.Count == 2)
                {
                    var input1 = BuildFormula(component.InputWires[0], components, wireToOutputComponent, inputToggleNames);
                    var input2 = BuildFormula(component.InputWires[1], components, wireToOutputComponent, inputToggleNames);
                    return $"(({input1}&{input2})|(!{input1}&!{input2}))";
                }
                else
                {
                    // For multiple inputs, it's the negation of XOR
                    var inputs = component.InputWires.Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames)).ToList();
                    var xorResult = inputs[0];
                    for (int i = 1; i < inputs.Count; i++)
                    {
                        xorResult = $"(({xorResult}&!{inputs[i]})|(!{xorResult}&{inputs[i]}))";
                    }
                    return $"!({xorResult})";
                }
                
            default:
                return "Unknown";
        }
    }
    
    private static List<string> ExtractInputVariables(string formula)
    {
        var inputs = new HashSet<string>();
        var tokens = formula.Split(new[] { ' ', '(', ')', '&', '|', '!' }, StringSplitOptions.RemoveEmptyEntries);
        
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