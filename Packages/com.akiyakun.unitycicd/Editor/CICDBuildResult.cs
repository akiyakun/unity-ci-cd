using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityCICD.Editor
{
    public class CICDBuildResult
    {
        public bool BuildSucceeded = false;
        public string BuildDirectory = "";

        public static CICDBuildResult CreateFailed()
        {
            return new CICDBuildResult
            {
                BuildSucceeded = false,
            };
        }
    }
}
