using System;

namespace ResourceAllocationTool.Services
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}