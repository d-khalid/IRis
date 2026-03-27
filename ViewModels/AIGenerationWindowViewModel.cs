using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IRis.Services;
using IRis.Views;       // just ignore these useless uses lol
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IRis.ViewModels
{
    public partial class AIGenerationWindowViewModel : ViewModelBase
    {
        public event Action<string>? XmlGenerated;

        private IAiPromptAnalysisService aiPromptAnalysisService = new GptAiAnalysisService();

        AIGenerationWindow promptWindow;

        [ObservableProperty]
        private String promptText = "NONE";

        public AIGenerationWindowViewModel(AIGenerationWindow promptWindow)
        {
            GenerateCommand = new AsyncRelayCommand(Generate);



            this.promptWindow = promptWindow;       // get the prompt window for use in this scope
        }

        public ICommand GenerateCommand { get; }
        public async Task Generate()
        {
            Console.WriteLine("Generate {PromptText}");

            // Relative Path            XmlGenerated.Invoke(xml);

            string xml = await aiPromptAnalysisService.GetSerializedCircuit(PromptText, "circuit-gen-prompt.txt");

            // Invoke event when Xml is done
            XmlGenerated?.Invoke(xml);

            Console.WriteLine("\n\nXML:\n" + xml);

            promptWindow.Close();
        }
    }

}