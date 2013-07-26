﻿using Newtonsoft.Json.Linq;
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
        public static String CLIPBOARD_URL;

        public static ClipUser GetUser(String id)
        {
            RestClient client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));

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

                        ClipUser clipUser = new ClipUser();
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

        public static ClipUser Authenticate(String name, String password)
        {
            RestClient client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));

            RestRequest request = new RestRequest("users", Method.GET);
            request.AddParameter("name", name);
            request.AddParameter("password", EncryptionHelper.Hash(password, "gdog"));

            try
            {
                RestResponse response = (RestResponse)client.Execute(request);

                if (response.ResponseStatus == ResponseStatus.Completed)
                {
                    String returnContent = response.Content;

                    if ((returnContent != null) && (returnContent != "null"))
                    {
                        JObject user = JObject.Parse(returnContent);

                        ClipUser clipUser = new ClipUser();
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

        public static ClipUser CreateUser(String name, String password)
        {
            RestClient client = new RestClient(CLIPBOARD_URL.TrimEnd(new char[] { '/' }));

            RestRequest request = new RestRequest("users", Method.POST);
            request.AddParameter("name", name);
            request.AddParameter("password", EncryptionHelper.Hash(password, "gdog"));

            try
            {
                RestResponse response = (RestResponse)client.Execute(request);

                String returnContent = response.Content.Replace("\"", String.Empty);

                if ((returnContent != null) && (returnContent != "null"))
                {
                    ClipUser clipUser = new ClipUser();
                    clipUser.Id = returnContent;
                    clipUser.Name = name;

                    return clipUser;
                }
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}