// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Xml.Linq;
// using System.Text.RegularExpressions;

// namespace IRis.Services;
// public class CircuitFormulaConversionService
// {
//     public class CircuitComponent
//     {
//         public required string Id { get; set; }
//         public required string Type { get; set; }
//         public List<string> InputWires { get; set; } = new List<string>();
//         public string OutputWire { get; set; } = null!;
//         public string Value { get; set; } = null!; // For LogicToggle components
//     }

//     public class CircuitFormula
//     {
//         public required string OutputName { get; set; }
//         public required string Formula { get; set; }
//         public List<string> InputVariables { get; set; } = new List<string>();
//     }

//     public static List<CircuitFormula> ConvertXmlToFormulas(string xmlFilePath)
//     {
//         string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
//         return ConvertXmlContentToFormulas(xmlContent);
//     }

//     public static List<CircuitFormula> ConvertXmlContentToFormulas(string xmlContent)
//     {
//         XDocument doc = XDocument.Parse(xmlContent);
        
//         var components = new Dictionary<string, CircuitComponent>();
//         var wireToOutputComponent = new Dictionary<string, string>();
//         var wireToInputComponents = new Dictionary<string, List<string>>();
//         var inputToggleNames = new Dictionary<string, string>();
//         var allWireIds = new HashSet<string>();
//         var connectedWireIds = new HashSet<string>();
        
//         // First pass: collect all wire IDs and identify connected wires
//         foreach (var componentEl in doc.Descendants("Component"))
//         {
//             var terminals = componentEl.Descendants("Terminal").ToList();
//             foreach (var terminal in terminals)
//             {
//                 var wireIds = terminal.Descendants("guid").Select(g => g.Value).ToList();
//                 foreach (var wireId in wireIds)
//                 {
//                     allWireIds.Add(wireId);
                    
//                     // Count how many terminals this wire connects to
//                     int connectionCount = doc.Descendants("Terminal")
//                         .Count(t => t.Descendants("guid").Any(g => g.Value == wireId));
                    
//                     // A wire is considered connected if it connects to more than one terminal
//                     if (connectionCount > 1)
//                     {
//                         connectedWireIds.Add(wireId);
//                     }
//                 }
//             }
//         }
        
//         // Parse components, but only process those connected to valid wires
//         foreach (var componentEl in doc.Descendants("Component"))
//         {
//             string type = componentEl.Attribute("Type")?.Value!;
//             string componentId = Guid.NewGuid().ToString(); // Generate unique ID for component
            
//             var component = new CircuitComponent
//             {
//                 Id = componentId,
//                 Type = type
//             };
            
//             // Get terminals and connected wires
//             var terminals = componentEl.Descendants("Terminal").ToList();
//             bool hasValidConnections = false;
            
//             if (type == "LogicToggle")
//             {
//                 // For input toggles, the wire is an output
//                 var wireIds = terminals.FirstOrDefault()?.Descendants("guid").Select(g => g.Value).ToList();
//                 if (wireIds?.Any() == true && connectedWireIds.Contains(wireIds.First()))
//                 {
//                     component.OutputWire = wireIds.First();
//                     wireToOutputComponent[wireIds.First()] = componentId;
                    
//                     // Create a meaningful name for this input
//                     string inputName = $"Input_{inputToggleNames.Count + 1}";
//                     inputToggleNames[componentId] = inputName;
//                     hasValidConnections = true;
//                 }
//             }
//             else if (type == "LogicProbe")
//             {
//                 // For output probes, the wire is an input
//                 var wireIds = terminals.FirstOrDefault()?.Descendants("guid").Select(g => g.Value).ToList();
//                 if (wireIds?.Any() == true && connectedWireIds.Contains(wireIds.First()))
//                 {
//                     component.InputWires.Add(wireIds.First());
//                     if (!wireToInputComponents.ContainsKey(wireIds.First()))
//                         wireToInputComponents[wireIds.First()] = new List<string>();
//                     wireToInputComponents[wireIds.First()].Add(componentId);
//                     hasValidConnections = true;
//                 }
//             }
//             else
//             {
//                 // For logic gates, first terminals are inputs, last is output
//                 for (int i = 0; i < terminals.Count - 1; i++)
//                 {
//                     var wireIds = terminals[i].Descendants("guid").Select(g => g.Value).ToList();
//                     foreach (var wireId in wireIds)
//                     {
//                         if (connectedWireIds.Contains(wireId))
//                         {
//                             component.InputWires.Add(wireId);
//                             if (!wireToInputComponents.ContainsKey(wireId))
//                                 wireToInputComponents[wireId] = new List<string>();
//                             wireToInputComponents[wireId].Add(componentId);
//                             hasValidConnections = true;
//                         }
//                     }
//                 }
                
//                 // Last terminal is output
//                 if (terminals.Count > 0)
//                 {
//                     var outputWireIds = terminals.Last().Descendants("guid").Select(g => g.Value).ToList();
//                     if (outputWireIds.Any() && connectedWireIds.Contains(outputWireIds.First()))
//                     {
//                         component.OutputWire = outputWireIds.First();
//                         wireToOutputComponent[outputWireIds.First()] = componentId;
//                         hasValidConnections = true;
//                     }
//                 }
//             }
            
//             // Only add component if it has valid connections
//             if (hasValidConnections)
//             {
//                 components[componentId] = component;
//             }
//         }
        
//         // Generate formulas for each output probe
//         var formulas = new List<CircuitFormula>();
//         var outputProbes = components.Values.Where(c => c.Type == "LogicProbe").ToList();
        
//         for (int i = 0; i < outputProbes.Count; i++)
//         {
//             var probe = outputProbes[i];
//             string outputName = $"Output_{i + 1}";
            
//             // Add safety check here
//             if (!probe.InputWires.Any())
//             {
//                 Console.WriteLine($"Warning: Output probe {i + 1} has no input wires connected.");
//                 continue; // Skip this probe or provide a default formula
//             }
            
//             string formula = BuildFormula(probe.InputWires.First(), components, wireToOutputComponent, inputToggleNames);
            
//             // Simplify the formula to remove redundant operations
//             formula = SimplifyFormula(formula);
            
//             var circuitFormula = new CircuitFormula
//             {
//                 OutputName = outputName,
//                 Formula = formula,
//                 InputVariables = ExtractInputVariables(formula)
//             };
            
//             formulas.Add(circuitFormula);
//         }
        
//         return formulas;
//     }

//     public static int GetNumberOfInputs(string xmlContent)
//     {
//         XDocument doc = XDocument.Parse(xmlContent);
//         return doc.Descendants("Component")
//                   .Count(c => c.Attribute("Type")?.Value == "LogicToggle");
//     }

//     public static int GetNumberOfInputsFromFile(string xmlFilePath)
//     {
//         string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
//         return GetNumberOfInputs(xmlContent);
//     }

//     public static int GetNumberOfOutputs(string xmlContent)
//     {
//         XDocument doc = XDocument.Parse(xmlContent);
//         return doc.Descendants("Component")
//                   .Count(c => c.Attribute("Type")?.Value == "LogicProbe");
//     }

//     public static int GetNumberOfOutputsFromFile(string xmlFilePath)
//     {
//         string xmlContent = System.IO.File.ReadAllText(xmlFilePath);
//         return GetNumberOfOutputs(xmlContent);
//     }

//     /// <summary>
//     /// Evaluates a boolean formula with given input values
//     /// </summary>
//     /// <param name="formula">The boolean formula string (using &, |, !, ^ operators)</param>
//     /// <param name="inputs">Dictionary mapping input variable names to their boolean values</param>
//     /// <returns>The boolean result of the formula evaluation</returns>
//     public static bool EvaluateFormula(string formula, Dictionary<string, bool> inputs)
//     {
//         if (string.IsNullOrWhiteSpace(formula))
//             return false;

//         // Handle simple cases
//         if (formula == "0") return false;
//         if (formula == "1") return true;

//         // Replace input variables with their values
//         string expression = formula;
//         foreach (var input in inputs)
//         {
//             string pattern = @"\b" + Regex.Escape(input.Key) + @"\b";
//             expression = Regex.Replace(expression, pattern, input.Value ? "1" : "0");
//         }

//         // Evaluate the boolean expression
//         return EvaluateBooleanExpression(expression);
//     }

//     private static bool EvaluateBooleanExpression(string expression)
//     {
//         // Remove whitespace
//         expression = expression.Replace(" ", "");

//         // Handle parentheses first (recursive evaluation)
//         while (expression.Contains("("))
//         {
//             int lastOpen = expression.LastIndexOf('(');
//             int firstClose = expression.IndexOf(')', lastOpen);
            
//             if (firstClose == -1)
//                 throw new ArgumentException("Mismatched parentheses in expression");

//             string subExpression = expression.Substring(lastOpen + 1, firstClose - lastOpen - 1);
//             bool subResult = EvaluateBooleanExpression(subExpression);
            
//             expression = expression.Substring(0, lastOpen) + (subResult ? "1" : "0") + expression.Substring(firstClose + 1);
//         }

//         // Handle NOT operations first (highest precedence)
//         while (expression.Contains("!"))
//         {
//             int notIndex = expression.IndexOf('!');
//             if (notIndex == expression.Length - 1)
//                 throw new ArgumentException("Invalid expression: NOT operator at end");

//             char nextChar = expression[notIndex + 1];
//             bool valueToNegate = nextChar == '1';
            
//             expression = expression.Substring(0, notIndex) + (valueToNegate ? "0" : "1") + expression.Substring(notIndex + 2);
//         }

//         // Handle XOR operations (before AND/OR)
//         while (expression.Contains("^"))
//         {
//             int xorIndex = expression.IndexOf('^');
//             if (xorIndex == 0 || xorIndex == expression.Length - 1)
//                 throw new ArgumentException("Invalid expression: XOR operator at boundary");

//             bool left = expression[xorIndex - 1] == '1';
//             bool right = expression[xorIndex + 1] == '1';
//             bool result = left ^ right;

//             expression = expression.Substring(0, xorIndex - 1) + (result ? "1" : "0") + expression.Substring(xorIndex + 2);
//         }

//         // Handle AND operations (higher precedence than OR)
//         while (expression.Contains("&"))
//         {
//             int andIndex = expression.IndexOf('&');
//             if (andIndex == 0 || andIndex == expression.Length - 1)
//                 throw new ArgumentException("Invalid expression: AND operator at boundary");

//             bool left = expression[andIndex - 1] == '1';
//             bool right = expression[andIndex + 1] == '1';
//             bool result = left && right;

//             expression = expression.Substring(0, andIndex - 1) + (result ? "1" : "0") + expression.Substring(andIndex + 2);
//         }

//         // Handle OR operations (lowest precedence)
//         while (expression.Contains("|"))
//         {
//             int orIndex = expression.IndexOf('|');
//             if (orIndex == 0 || orIndex == expression.Length - 1)
//                 throw new ArgumentException("Invalid expression: OR operator at boundary");

//             bool left = expression[orIndex - 1] == '1';
//             bool right = expression[orIndex + 1] == '1';
//             bool result = left || right;

//             expression = expression.Substring(0, orIndex - 1) + (result ? "1" : "0") + expression.Substring(orIndex + 2);
//         }

//         // Final result should be a single digit
//         if (expression.Length != 1 || (expression != "0" && expression != "1"))
//             throw new ArgumentException($"Invalid expression result: {expression}");

//         return expression == "1";
//     }

//     /// <summary>
//     /// Simplifies boolean formulas by removing redundant operations and constants
//     /// </summary>
//     private static string SimplifyFormula(string formula)
//     {
//         if (string.IsNullOrWhiteSpace(formula))
//             return "0";

//         string simplified = formula;
//         string previous;
        
//         // Keep simplifying until no more changes occur
//         do
//         {
//             previous = simplified;
            
//             // Remove operations with 0 that result in known values
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)&0\)", "0"); // X & 0 = 0
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(0&([^()]+)\)", "0"); // 0 & X = 0
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)\|0\)", "$1"); // X | 0 = X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(0\|([^()]+)\)", "$1"); // 0 | X = X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)\^0\)", "$1"); // X ^ 0 = X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(0\^([^()]+)\)", "$1"); // 0 ^ X = X
            
//             // Remove operations with 1 that result in known values
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)&1\)", "$1"); // X & 1 = X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(1&([^()]+)\)", "$1"); // 1 & X = X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)\|1\)", "1"); // X | 1 = 1
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(1\|([^()]+)\)", "1"); // 1 | X = 1
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()]+)\^1\)", "(!$1)"); // X ^ 1 = !X
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(1\^([^()]+)\)", "(!$1)"); // 1 ^ X = !X
            
//             // Simplify double negation
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"!\(!([^()]+)\)", "$1"); // !!X = X
            
//             // Remove unnecessary parentheses around single terms
//             simplified = System.Text.RegularExpressions.Regex.Replace(simplified, @"\(([^()&|^!]+)\)", "$1");
            
//         } while (simplified != previous && !string.IsNullOrEmpty(simplified));
        
//         return string.IsNullOrEmpty(simplified) ? "0" : simplified;
//     }
    
//     private static string BuildFormula(string wireId, Dictionary<string, CircuitComponent> components, 
//         Dictionary<string, string> wireToOutputComponent, Dictionary<string, string> inputToggleNames)
//     {
//         if (!wireToOutputComponent.ContainsKey(wireId))
//             return "0"; // Wire not connected to any output
            
//         string componentId = wireToOutputComponent[wireId];
//         var component = components[componentId];
        
//         switch (component.Type)
//         {
//             case "LogicToggle":
//                 return inputToggleNames[componentId];
                
//             case "XorGate":
//                 // XOR: A ^ B
//                 var xorInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (xorInputs.Count == 0) return "0";
//                 if (xorInputs.Count == 1) return xorInputs[0];
                
//                 var xorResult = xorInputs[0];
//                 for (int i = 1; i < xorInputs.Count; i++)
//                 {
//                     xorResult = $"({xorResult}^{xorInputs[i]})";
//                 }
//                 return xorResult;
                
//             case "AndGate":
//                 var andInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (andInputs.Count == 0) return "0";
//                 if (andInputs.Count == 1) return andInputs[0];
                
//                 return $"({string.Join("&", andInputs)})";
                
//             case "OrGate":
//                 var orInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (orInputs.Count == 0) return "0";
//                 if (orInputs.Count == 1) return orInputs[0];
                
//                 return $"({string.Join("|", orInputs)})";

//             case "NotGate":
//                 if (!component.InputWires.Any())
//                     return "1"; // NOT gate with no input defaults to 1 (NOT 0 = 1)
                    
//                 var notInput = BuildFormula(component.InputWires.First(), components, wireToOutputComponent, inputToggleNames);
//                 if (notInput == "0") return "1";
//                 return $"(!{notInput})";

//             case "NandGate":
//                 // NAND: !(A & B & C...)
//                 var nandInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (nandInputs.Count == 0) return "1"; // NAND with no inputs = !0 = 1
//                 if (nandInputs.Count == 1) return $"(!{nandInputs[0]})";
                
//                 return $"!({string.Join("&", nandInputs)})";

//             case "NorGate":
//                 // NOR: !(A | B | C...)
//                 var norInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (norInputs.Count == 0) return "1"; // NOR with no inputs = !0 = 1
//                 if (norInputs.Count == 1) return $"(!{norInputs[0]})";
                
//                 return $"!({string.Join("|", norInputs)})";

//             case "XnorGate":
//                 // XNOR: !(A ^ B)
//                 var xnorInputs = component.InputWires
//                     .Select(w => BuildFormula(w, components, wireToOutputComponent, inputToggleNames))
//                     .Where(f => f != "0") // Filter out unconnected inputs
//                     .ToList();
                
//                 if (xnorInputs.Count == 0) return "1"; // XNOR with no inputs = !0 = 1
//                 if (xnorInputs.Count == 1) return $"(!{xnorInputs[0]})"; // XNOR of single input = NOT input
                
//                 var xnorResult = xnorInputs[0];
//                 for (int i = 1; i < xnorInputs.Count; i++)
//                 {
//                     xnorResult = $"({xnorResult}^{xnorInputs[i]})";
//                 }
//                 return $"!({xnorResult})";
                
//             default:
//                 return "Unknown";
//         }
//     }
    
//     private static List<string> ExtractInputVariables(string formula)
//     {
//         var inputs = new HashSet<string>();
//         var tokens = formula.Split(new[] { ' ', '(', ')', '&', '|', '!', '^' }, StringSplitOptions.RemoveEmptyEntries);
        
//         foreach (var token in tokens)
//         {
//             if (token.StartsWith("Input_"))
//             {
//                 inputs.Add(token);
//             }
//         }
        
//         return inputs.OrderBy(x => x).ToList();
//     }
// }