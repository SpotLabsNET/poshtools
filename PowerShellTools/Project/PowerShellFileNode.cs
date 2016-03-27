﻿using System;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudioTools.Project;

namespace PowerShellTools.Project
{
    internal class PowerShellFileNode : CommonFileNode
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellFileNode"/> class.
        /// </summary>
        /// <param name="root">The project node.</param>
        /// <param name="e">The project element node.</param>
        internal PowerShellFileNode(CommonProjectNode root, ProjectElement e)
            : base(root, e)
        {
        }
        #endregion

        protected override NodeProperties CreatePropertiesObject()
        {
            return new PowerShellFileNodeProperties(this);
        }

        public override int ImageIndex
        {
            get
            {
                if (ItemNode.IsExcluded)
                {
                    return (int)ProjectNode.ImageName.ExcludedFile;
                }
                else if (!File.Exists(Url))
                {
                    return (int)ProjectNode.ImageName.MissingFile;
                }
                else if (IsFormSubType)
                {
                    return (int)ProjectNode.ImageName.WindowsForm;
                }
                else if (this.ProjectMgr.IsCodeFile(FileName))
                {
                    ImageListIndex index = ImageListIndex.Script;

                    if (FileName.EndsWith(PowerShellConstants.PSM1File, StringComparison.OrdinalIgnoreCase))
                    {
                        index = ImageListIndex.Module;
                    }
                    else if (FileName.EndsWith(PowerShellConstants.PSD1File, StringComparison.OrdinalIgnoreCase))
                    {
                        index = ImageListIndex.DataFile;
                    }
                    else if (FileName.EndsWith(PowerShellConstants.Test, StringComparison.OrdinalIgnoreCase))
                    {
                        index = ImageListIndex.Test;
                    }

                    return CommonProjectNode.ImageOffset + (int)index;
                }

                return base.ImageIndex;
            }
        }

        internal override int QueryStatusOnNode(Guid guidCmdGroup, uint cmd, IntPtr pCmdText, ref QueryStatusResult result)
        {
            if (guidCmdGroup == VsMenus.guidStandardCommandSet97 && IsFormSubType)
            {
                switch ((VSConstants.VSStd97CmdID)cmd)
                {
                    case VSConstants.VSStd97CmdID.ViewForm:
                        result |= QueryStatusResult.SUPPORTED | QueryStatusResult.ENABLED;
                        return VSConstants.S_OK;
                }
            }

            return base.QueryStatusOnNode(guidCmdGroup, cmd, pCmdText, ref result);
        }
    }
}
