﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Core.QueryBuilder.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using QueryBuilder.Templates;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\UpdateCalendarEntity.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class UpdateCalendarEntity : TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\nUPDATE [");
            
            #line 8 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\UpdateCalendarEntity.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.vwProjection.DbObject_Schema));
            
            #line default
            #line hidden
            this.Write("].[");
            
            #line 8 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\UpdateCalendarEntity.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.vwProjection.DbObject_Name));
            
            #line default
            #line hidden
            this.Write("]\r\nSET [");
            
            #line 9 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\UpdateCalendarEntity.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.vwProjection.EventStartColumnName));
            
            #line default
            #line hidden
            this.Write("] = @startDate, [");
            
            #line 9 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\UpdateCalendarEntity.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(this.vwProjection.EventEndColumnName));
            
            #line default
            #line hidden
            this.Write("] = @endDate\r\nWHERE [Id] = @entityId\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
