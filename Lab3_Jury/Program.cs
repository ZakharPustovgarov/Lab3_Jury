using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using Utility;

namespace Lab3_Jury
{
    class Program
    {
        const string okAnswer = "HttpStatusCode.Ok (200)";
        const string baseUri = "http://127.0.0.1:";
        const string pingMethod = "Ping/";
        const string postInputMethod = "PostInputData/";
        const string getAnswerMethod = "GetAnswer/";
        const string stopMethod = "Stop/";

        const string defaultConsoleMessage = "Write command number:\n1 - " + pingMethod +
            "\n2 - " + postInputMethod +
            "\n3 - " + getAnswerMethod +
            "\n4 - " + stopMethod;


        private async static Task Main()
        {
            int port = -1;

            Console.WriteLine("Please, type necessary port below");
            port = Convert.ToInt32(Console.ReadLine());

            string currentUri = baseUri + port + "/";

            HttpClientHandler clientHandler = new HttpClientHandler();
            HttpClient httpClient = new HttpClient(clientHandler);

            int option = -1;

            bool isWorking = true;

            Input input = new Input();
            input.K = 5;
            input.Muls = new int[] { 6, 4, 5 };
            input.Sums = new decimal[] { 1.1m, 4.5m, 5.6745m };

            Output output = new Output(input);
            HttpResponseMessage answer = null;
            StringContent content;
           
            bool isOK = false;

            while (!isOK)
            {
                string str = await httpClient.GetStringAsync(currentUri + pingMethod);

                //content = (StringContent);

                //string str = answer.Content.ToString();

                if (str == okAnswer) isOK = true;

                Thread.Sleep(5);
            }

            while (isWorking)
            {
                Console.WriteLine(defaultConsoleMessage);

                option = Convert.ToInt32(Console.ReadLine());

                string bufStr = "";

                switch (option)
                {
                    case 1:
                        bufStr = await httpClient.GetStringAsync(currentUri + pingMethod);
                        Console.WriteLine(bufStr);
                        break;
                    case 2:
                        content = new StringContent(input.SerializeInput());
                        //content = new StringContent("Kek");
                        httpClient.PostAsync(currentUri + postInputMethod, content);
                        break;
                    case 3:
                        bufStr = await httpClient.GetStringAsync(currentUri + getAnswerMethod);
                        Output newOutput = new Output(bufStr);
                        bool isCorrect = CheckAnswer(output, newOutput);
                        if (isCorrect) Console.WriteLine("Recieved correct answer!");
                        else Console.WriteLine("Recieved incorrect answer...");
                        break;
                    case 4:
                        httpClient.GetStringAsync(currentUri + stopMethod);
                        Console.WriteLine(bufStr);
                        break;
                    case 0:
                        isWorking = false;
                        break;
                }
            }
        }

        private static bool CheckAnswer(Output neededOutput, Output recievedOutput)
        {
            if (neededOutput.SumResult != recievedOutput.SumResult) return false;
            else if (neededOutput.MulResult != recievedOutput.MulResult) return false;
            else if (neededOutput.SortedInputs.Length != recievedOutput.SortedInputs.Length) return false;
            else
            {
                for(int i = 0; i < neededOutput.SortedInputs.Length; i++)
                {
                    if (neededOutput.SortedInputs[i] != recievedOutput.SortedInputs[i]) return false;
                }

                return true;
            }    
        }
    }
}
