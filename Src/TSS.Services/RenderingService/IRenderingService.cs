using System;
using TSS.Domain;

namespace TSS.Services
{
    public interface IRenderingService
    {
        string UpdateKey();
        string GetContentToken(ContentRequest request);
    }
}
