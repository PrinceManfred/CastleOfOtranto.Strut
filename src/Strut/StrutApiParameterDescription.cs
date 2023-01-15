using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace CastleOfOtranto.Strut;

public class StrutApiParameterDescription : ApiParameterDescription
{
    public bool StrutIsRequired { get; set; }
}

