using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Skywriter.Model;
using Skywriter.Helpers;

namespace Skywriter.Webservices
{
    public class UserWebservices
    {
        private static readonly String PASSWORD_SALT = "gdog";

        public static String CLIPBOARD_URL;

        public static SkywriterUser GetUser(String id)
        {
            RestClient client;

            try
            {
                client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));
            }
            catch (Exception ex)
            {
                return null;
            }

            RestRequest request = new RestRequest("users", Method.GET);
            request.AddParameter("id", id);

            try
            {
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    String returnContent = response.Content;

                    if ((returnContent != null) && (returnContent != "null"))
                    {
                        JObject result = JObject.Parse(returnContent);

                        JObject user = JObject.Parse(returnContent);

                        SkywriterUser clipUser = new SkywriterUser();
                        clipUser.Id = user["Id"].ToString();
                        clipUser.Name = user["Name"].ToString();

                        return clipUser;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        public static SkywriterUser Authenticate(String name, String password, out LoginUserResult loginUserResult)
        {
            RestClient client;

            try
            {
                client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));
            }
            catch (Exception ex)
            {
                loginUserResult = LoginUserResult.ErroredConnection;
                return null;
            }

            RestRequest request = new RestRequest("users", Method.GET);
            request.AddParameter("name", name);
            request.AddParameter("password", EncryptionHelper.Hash(password, PASSWORD_SALT));

            try
            {
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    String returnContent = response.Content;

                    if ((returnContent != null) && (returnContent != "null"))
                    {
                        JObject user = JObject.Parse(returnContent);

                        SkywriterUser clipUser = new SkywriterUser();
                        clipUser.Id = user["Id"].ToString();
                        clipUser.Name = user["Name"].ToString();

                        loginUserResult = LoginUserResult.Successful;

                        return clipUser;
                    }
                    else
                    {
                        loginUserResult = LoginUserResult.NotFound;
                        return null;
                    }
                }
                else
                {
                    loginUserResult = LoginUserResult.ErroredConnection;
                    return null;
                }
            }
            catch (Exception ex)
            {
                loginUserResult = LoginUserResult.ErroredConnection;
                return null;
            }
        }

        public static SkywriterUser CreateUser(String name, String password, out CreateUserResult createUserResult)
        {
            RestClient client;

            try
            {
                client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));
            }
            catch (Exception ex)
            {
                createUserResult = CreateUserResult.ErroredConnection;
                return null;
            }

            RestRequest request = new RestRequest("users", Method.POST);
            request.AddParameter("name", name);
            request.AddParameter("password", EncryptionHelper.Hash(password, PASSWORD_SALT));

            try
            {
                RestResponse response = (RestResponse)client.Execute(request);

                String returnContent = response.Content.Replace("\"", String.Empty);

                if ((returnContent != null) && (returnContent != "null"))
                {
                    SkywriterUser clipUser = new SkywriterUser();
                    clipUser.Id = returnContent;
                    clipUser.Name = name;

                    createUserResult = CreateUserResult.Successful;

                    return clipUser;
                }
                else
                {
                    createUserResult = CreateUserResult.AlreadyExisting;
                    return null;
                }
            }
            catch (Exception ex)
            {
                createUserResult = CreateUserResult.ErroredConnection;
                return null;
            }
        }
    }
}
