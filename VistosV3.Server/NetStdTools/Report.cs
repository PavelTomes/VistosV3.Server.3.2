using NetStdTools.ReportExecution;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NetStdTools
{
    public class ReportGenerator
    {
        private string url = null;
        private string userName = null;
        private string password = null;

        public ReportGenerator(string url, string userName, string password)
        {
            this.url = url;
            this.userName = userName;
            this.password = password;
        }

        public async Task<byte[]> RenderReport(string report, ParameterValue[] parameters, string exportFormat = "PDF")
        {
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;
            binding.MaxReceivedMessageSize = 100485760;

            var rsExec = new ReportExecutionServiceSoapClient(binding, new EndpointAddress(url));

            var clientCredentials = new NetworkCredential(userName, password);
            rsExec.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            rsExec.ClientCredentials.Windows.ClientCredential = clientCredentials;

            TrustedUserHeader trustedUserHeader = new TrustedUserHeader();

            LoadReportResponse taskLoadReport = null;
            string historyID = null;
            taskLoadReport = await rsExec.LoadReportAsync(trustedUserHeader, report, historyID);

            ExecutionHeader executionHeader = new ExecutionHeader
            {
                ExecutionID = taskLoadReport.executionInfo.ExecutionID
            };

            await rsExec.SetExecutionParametersAsync(executionHeader, trustedUserHeader, parameters, null);
            const string deviceInfo = @"<DeviceInfo><Toolbar>False</Toolbar><SimplePageHeaders>True</SimplePageHeaders></DeviceInfo>";
            RenderResponse response = await rsExec.RenderAsync(new RenderRequest(executionHeader, trustedUserHeader, exportFormat ?? "PDF", deviceInfo));

            return response.Result;
        }
    }


}
