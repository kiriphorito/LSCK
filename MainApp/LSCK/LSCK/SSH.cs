using System;
using System.IO;
using Renci.SshNet;

namespace LSCK
{
    public class SSH
    {
        string generateDir;

        public SSH(string generateDir)
        {
            this.generateDir = generateDir;
        }

        public ConnectionInfo CreateConnectionInfo(string ip, string username, string privateKeyFilePath)
        {
            ConnectionInfo connectionInfo;
            using (var stream = new FileStream(privateKeyFilePath, FileMode.Open, FileAccess.Read))
            {
                var privateKeyFile = new PrivateKeyFile(stream);
                AuthenticationMethod authenticationMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
                connectionInfo = new ConnectionInfo(ip, username, authenticationMethod);
            }
            return connectionInfo;
        }


        public void UploadWebsite()
        {
            using (var ssh = new SshClient(CreateConnectionInfo("52.56.181.183", "ubuntu", @"C:\Users\Sam\Downloads\comp205p.pem")))
            {
                ssh.Connect();
                var command = ssh.CreateCommand("rm -r *");
                var result = command.Execute();
                Console.Out.WriteLine(result);
                ssh.Disconnect();
            }
            using (var scp = new ScpClient(CreateConnectionInfo("52.56.181.183", "ubuntu", @"C:\Users\Sam\Downloads\comp205p.pem")))
            {
                scp.Connect();
                scp.Upload(new DirectoryInfo(generateDir), "/var/www/html/");
                scp.Disconnect();
            }
        }
    }
}
