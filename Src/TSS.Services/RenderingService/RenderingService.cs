using System;
using System.IO;
using System.Net;
using TSS.Data;
using TSS.Domain;
namespace TSS.Services
{
    public class RenderingService : IRenderingService
    {
        private readonly IRenderingApi _renderingApi;

        public RenderingService(IRenderingApi renderingApi)
        {
            _renderingApi = renderingApi;
        }

        /// <summary>
        /// Retrieving latest API key used for rendering
        /// </summary>
        /// <returns></returns>
        public string UpdateKey()
        {
            var key = _renderingApi.GetKey();
            return key;

        }

        private static int foo = 0;
        /// <summary>
        /// Create an encrypted request packet for content rendering
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string GetContentToken(ContentRequest content)
        {
            try
            {
                string key = UpdateKey();
                var encryptedContent = _renderingApi.EncryptContentData(content, key);
                string jwtToken = _renderingApi.SignWithJWT(encryptedContent);

                // Unit test code for key regeneration... 
                /* foo = foo+1;
                if (foo%3 == 0)
                {
                    throw (new Exception("exception - do we redo the token?"));
                }  */
                return jwtToken;
            }
            catch (Exception exp)
            {
                LoggerRepository.Instance.LogException(exp);
                _renderingApi.ForceExpire();
                throw exp;
            }
            //return _renderingApi.GetContent(jwtToken);
        }

    }
}
