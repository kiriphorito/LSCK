// <copyright file="SitePreview.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>

namespace LSCK
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid("b325ff0b-8b2d-42cb-8fd2-5f6a9c21a998")]
    public class SitePreview : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SitePreview"/> class.
        /// </summary>
        public SitePreview() : base(null)
        {
            this.Caption = "SitePreview";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new SitePreviewControl();
        }
    }
}
