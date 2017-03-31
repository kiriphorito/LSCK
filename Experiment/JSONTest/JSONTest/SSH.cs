using System;
using System.IO;
using Renci.SshNet;

namespace JSONTest
{
    public class SSH
    {
        string generateDir;

        public SSH(string generateDir)
        {
            this.generateDir = generateDir;
        }

        public ConnectionInfo CreateConnectionInfo()
        {
            const string privateKeyFilePath = @"C:\Users\Sam\Downloads\comp205p.pem";
            ConnectionInfo connectionInfo;
            using (var stream = new FileStream(privateKeyFilePath, FileMode.Open, FileAccess.Read))
            {
                var privateKeyFile = new PrivateKeyFile(stream);
                AuthenticationMethod authenticationMethod =
                    new PrivateKeyAuthenticationMethod("ubuntu", privateKeyFile);

                connectionInfo = new ConnectionInfo(
                    "52.56.181.183",
                    "ubuntu",
                    authenticationMethod);
            }

            return connectionInfo;
        }


        public void UploadWebsite()
        {
            using (var ssh = new SshClient(CreateConnectionInfo()))
            {
                ssh.Connect();
                var command = ssh.CreateCommand("rm -r *");
                var result = command.Execute();
                Console.Out.WriteLine(result);
                ssh.Disconnect();
            }
            using (var scp = new ScpClient(CreateConnectionInfo()))
            {
                scp.Connect();

                scp.Upload(new DirectoryInfo(generateDir), "/var/www/html/");

                scp.Disconnect();
            }
        }
    }
}
