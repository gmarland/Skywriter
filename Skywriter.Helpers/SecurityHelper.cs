using Skywriter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Skywriter.Helpers
{
    public class SecurityHelper
    {
        public static void DeleteUserDetails()
        {
            String detailsFile = AppDomain.CurrentDomain.BaseDirectory + "uDetail.bin";

            if (File.Exists(detailsFile))
            {
                File.Delete(detailsFile);
            }
        }

        public static void SerializeUserDetails(ClipUser clipUser)
        {
            String detailsFile = AppDomain.CurrentDomain.BaseDirectory + "uDetail.bin";

            if (File.Exists(detailsFile))
            {
                File.Delete(detailsFile);
            }

            using (FileStream fs = File.Create(detailsFile))
            {
                BinaryFormatter bf = new BinaryFormatter();

                try
                {
                    bf.Serialize(fs, clipUser);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public static ClipUser DeserializeUserDetails()
        {
            ClipUser clipUser = null;

            String detailsFile = AppDomain.CurrentDomain.BaseDirectory + "uDetail.bin";

            if (File.Exists(detailsFile))
            {
                using (FileStream fs = File.OpenRead(detailsFile))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    fs.Position = 0;

                    try
                    {
                        clipUser = (ClipUser)bf.Deserialize(fs);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }

            return clipUser;
        }
    }
}
