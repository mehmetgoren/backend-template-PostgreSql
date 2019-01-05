namespace Server.WebApi
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Connections.Internal;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.SignalR;

    public abstract class BaseHub : Hub
    {
        private static readonly Type TokenTableAuthAttributeType = typeof(ionix.Rest.TokenTableAuthAttribute);
        protected virtual bool CheckAuth(IFeatureCollection features)
        {
            if (this.GetType().GetCustomAttributes(TokenTableAuthAttributeType, false).FirstOrDefault() == null)
                return true;

            if (null == features)
                return false;

            foreach (var kvp in this.Context.Features)
            {
                if (kvp.Value is HttpConnectionContext httpContext)
                {
                    QueryString? qs = httpContext.HttpContext?.Request?.QueryString;
                    if (null != qs)
                    {
                        QueryString queryString = qs.Value;
                        if (queryString.HasValue)
                        {
                            string str = queryString.Value;
                            if (!String.IsNullOrEmpty(str))
                            {
                                if (str[0] == '?')
                                    str = str.Remove(0, 1);

                                NameValueCollection query = System.Web.HttpUtility.ParseQueryString(str);
                                string tokenStr = query["token"];
                                if (Guid.TryParse(tokenStr, out Guid token))
                                {
                                    return TokenTable.Instance.TryAuthenticateToken(token, out var user);//it does not auth api's. It is ok if user has been loggin. 
                                }
                            }
                        }
                    }

                    break;
                }
            }

            return false;

        }

        private static StackTrace stOnConnected;
        public override Task OnConnectedAsync()
        {
            if (!this.CheckAuth(this.Context.Features))
            {
                this.Context.Abort();
                return Task.Delay(0);
            }

            // Add your own code here.
            // For example: in a chat application, record the association between
            // the current connection ID and user name, and mark the user as online.
            // After the code in this method completes, the client is informed that
            // the connection is established; for example, in a JavaScript client,
            // the start().done callback is executed.
            if (this.Logging)
            {
                if (null == stOnConnected)
                    stOnConnected = new StackTrace();

                SQLog.Logger.Create(stOnConnected)
                    .Code(1010)
                    .Info($"Client (ClientId: {this.Context.ConnectionId}) was connected to Hub ({this.GetType()}:").SaveAsync();
            }
            return base.OnConnectedAsync();
        }

        private static StackTrace stOnDisconnected;

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Add your own code here.
            // For example: in a chat application, mark the user as offline, 
            // delete the association between the current connection id and user name.

            if (this.Logging)
            {
                if (null == stOnDisconnected)
                    stOnDisconnected = new StackTrace();

                SQLog.Logger.Create(stOnDisconnected)
                    .Code(1010)
                    .Info(
                        $"Client (ClientId: {this.Context.ConnectionId}) was disconnected to Hub ({this.GetType()}:")
                    .SaveAsync();
            }
            return base.OnDisconnectedAsync(exception);
        }

        protected bool Logging { get; set; } = true;

        public virtual Task JoinGroupAsync(string groupName)
        {
            return this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }

        public async Task JoinGroup(string groupName)
        {
            await this.JoinGroupAsync(groupName);
        }

        public virtual Task LeaveGroupAsync(string groupName)
        {
            return this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        }

        public async Task LeaveGroup(string groupName)
        {
            await this.LeaveGroupAsync(groupName);
        }
    }
}