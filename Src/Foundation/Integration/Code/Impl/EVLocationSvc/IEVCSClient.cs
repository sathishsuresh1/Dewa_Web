// <copyright file="IEVCSClient.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Integration.Impl.EVLocationSvc
{
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.EVLocationSvc;

    /// <summary>
    /// Defines the <see cref="IEVCSClient" />.
    /// </summary>
    public interface IEVCSClient
    {
        /// <summary>
        /// The GetLocations.
        /// </summary>
        /// <returns>The <see cref="ServiceResponse{Devicelist}"/>.</returns>
        ServiceResponse<Devicelist> GetLocations(string data = EVCSConstant.BASICDATA, string HubeleonID = "");
    }
}
