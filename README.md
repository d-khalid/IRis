# IRis - Circuit Simulator

IRis is an AI-powered circuit simulation software made with **Avalonia** and **C#**.

It uses [SketchLogic](https://github.com/shahzaibahmad05/sketchlogic) as backend to seemlessly generate simulatable circuits from sketches. 

<br>

<img src="https://drive.google.com/uc?export=view&id=1NguAusaeMhvOUsnOimggUou8rtfbO2aD" alt="Image">

<hr>

# Project Structure

````
IRis
├─ Assets/
│  └─ avalonia-logo.ico
├─ Models/
│  ├─ Components/
│  │  ├─ AndGate.cs
│  │  ├─ CustomComponent.cs
│  │  ├─ Decoder.cs
│  │  ├─ Demultiplexer.cs
│  │  ├─ DLatch.cs
│  │  ├─ Encoder.cs
│  │  ├─ JKLatch.cs
│  │  ├─ LogicProbe.cs
│  │  ├─ LogicToggle.cs
│  │  ├─ Multiplexer.cs
│  │  ├─ NandGate.cs
│  │  ├─ NorGate.cs
│  │  ├─ NotGate.cs
│  │  ├─ OrGate.cs
│  │  ├─ SRLatch.cs
│  │  ├─ TLatch.cs
│  │  ├─ Wire.cs
│  │  ├─ XnorGate.cs
│  │  └─ XorGate.cs
│  ├─ Core/
│  │  ├─ Component.cs
│  │  ├─ ComponentDefaults.cs
│  │  ├─ Gate.cs
│  │  └─ IOutputProvider.cs
│  ├─ DTOs/
│  │  ├─ CircuitDto.cs
│  │  ├─ ComponentDto.cs
│  │  ├─ TerminalDto.cs
│  │  └─ WireDto.cs
│  ├─ Commands.cs
│  ├─ ComponentManagers.cs
│  ├─ KeyGestureConfig.cs
│  ├─ PreviewManager.cs
│  ├─ SelectionManager.cs
│  └─ Simulation.cs
├─ Services/
│  ├─ AiImageAnalysisService.cs
│  ├─ CircuitFomulaConversionService.cs
│  ├─ GPTAiAnalysisService.cs
│  ├─ IAiAnalysisService.cs
│  ├─ ISerializationService.cs
│  └─ JsonSerializationService.cs
├─ ViewModels/
│  ├─ AIGenerationWindowViewModel.cs
│  ├─ ComponentPropertiesViewModel.cs
│  ├─ ExportComponentWindowViewModel.cs
│  ├─ ImageProcessingWindowViewModel.cs
│  ├─ MainWindowViewModel.cs
│  └─ ViewModelBase.cs
├─ Views/
│  ├─ AboutWindow.axaml
│  ├─ AboutWindow.axaml.cs
│  ├─ AIGenerationWindow.axaml
│  ├─ AIGenerationWindow.axaml.cs
│  ├─ ComponentPropertiesView.axaml
│  ├─ ComponentPropertiesView.axaml.cs
│  ├─ ExportComponentWindow.axaml
│  ├─ ExportComponentWindow.axaml.cs
│  ├─ ImageProcessingWindow.axaml
│  ├─ ImageProcessingWindow.cs
│  ├─ MainWindow.axaml
│  ├─ MainWindow.axaml.cs
│  ├─ OtherComponentsWindow.axaml
│  └─ OtherComponentsWindow.axaml.cs
├─ App.axaml
├─ App.axaml.cs
├─ app.manifest
├─ circuit-gen-prompt.txt
├─ IRis.csproj
├─ IRis.sln
├─ Program.cs
├─ README.md
├─ server-link.txt
└─ ViewLocator.cs
````

<hr>

# Build Instructions
1- Install .NET SDK 9.0 from [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0).  
2- Make sure it is installed properly by running `dotnet --version` in a terminal.  
3- Download the source code from here into a folder, open that folder in a terminal and execute `dotnet build`.  
4- .NET automatically resolves the dependencies, so it should build with no issues. Execute `dotnet run` to run the program.  

<hr>

# Local Backend Setup

For setting up the backend locally:

- Clone the repo [SketchLogic](https://github.com/shahzaibahmad05/sketchlogic).
- Refer to further instructions in [SketchLogic/backend/README](https://github.com/ShahzaibAhmad05/SketchLogic/blob/main/backend/README.md).

**Note:** Make sure to put *http://localhost:5000/* in `server-link.txt` to access the local API.
