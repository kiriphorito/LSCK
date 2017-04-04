using System.IO;
using Renci.SshNet;
using System.Windows.Forms;

namespace LSCK
{
    public class SSH
    {
        private string generateDir;

        public SSH(string generateDir)
        {
            this.generateDir = generateDir;
        }

        public ConnectionInfo CreateConnectionInfo(string ip, string username, string type, string details)
        {
            AuthenticationMethod authenticationMethod = null;
            switch (type)
            {
                case "pem":
                    string privateKeyFilePath = details;
                    using (var stream = new FileStream(privateKeyFilePath, FileMode.Open, FileAccess.Read))
                    {
                        var privateKeyFile = new PrivateKeyFile(stream);
                        authenticationMethod = new PrivateKeyAuthenticationMethod(username, privateKeyFile);
                    }
                    break;
                case "password":
                    authenticationMethod = new PasswordAuthenticationMethod(username, details);
                    break;
                default:
                    MessageBox.Show("Invalid authentication method");
                    break;
            }
            ConnectionInfo connectionInfo = new ConnectionInfo(ip, username, authenticationMethod);
            return connectionInfo;
        }


        public void UploadWebsite(string ip, string username, string type, string details)
        {
            using (var ssh = new SshClient(CreateConnectionInfo(ip, username, type, details)))
            {
                ssh.Connect();
                string result = ssh.CreateCommand("apache2 -v").Execute();
                if (!result.Contains("version"))
                {
                    MessageBox.Show("Installing Apache");
                    ssh.CreateCommand("sudo apt-get -y install apache2").Execute();
                    ssh.CreateCommand("sudo apt-get -y install php libapache2-mod-php").Execute();
                    ssh.CreateCommand("sudo /etc/init.d/apache2 restart").Execute();
                    ssh.CreateCommand("sudo chown " + username + " /var/www/html/").Execute();
                    MessageBox.Show("Installed Apache");
                }
                //else { MessageBox.Show("Apache Already Installed"); }
                ssh.CreateCommand("rm -r /var/www/html/*").Execute();
                ssh.Disconnect();
            }
            using (var scp = new ScpClient(CreateConnectionInfo(ip, username, type, details)))
            {
                scp.Connect();
                scp.Upload(new DirectoryInfo(generateDir), "/var/www/html/");
                scp.Disconnect();
            }
        }
    }
}