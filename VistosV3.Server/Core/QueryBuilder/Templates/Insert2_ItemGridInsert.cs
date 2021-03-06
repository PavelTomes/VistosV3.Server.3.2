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
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class Insert2_ItemGridInsert : Insert2_ItemGridInsertBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public virtual string TransformText()
        {
            
            #line 6 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"

PushIndent("				"); 

            
            #line default
            #line hidden
            this.Write("\r\ndeclare @rowQueueOrder");
            
            #line 10 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" int\r\ndeclare @newId");
            
            #line 11 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" int\r\ndeclare cursor");
            
            #line 12 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" CURSOR LOCAL FOR\r\nSELECT json");
            
            #line 13 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[QueueOrder]\r\nFROM OPENJSON(@json");
            
            #line 14 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("Rows)\r\nWITH ([Deleted] [bit], [QueueOrder] [int]) AS json");
            
            #line 15 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\nWHERE isnull(json");
            
            #line 16 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[Deleted], 0) = 0 AND json");
            
            #line 16 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[QueueOrder] > 0\r\n\r\nopen cursor");
            
            #line 18 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\nfetch next from cursor");
            
            #line 19 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" into @rowQueueOrder");
            
            #line 19 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\nwhile @@FETCH_STATUS=0\r\nbegin\r\n\t---");
            
            #line 22 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(relation.DbObject1_Name));
            
            #line default
            #line hidden
            this.Write(" insert\r\n\tINSERT INTO [");
            
            #line 23 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(relation.DbObject1_Schema));
            
            #line default
            #line hidden
            this.Write("].[");
            
            #line 23 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(relation.DbObject1_Name));
            
            #line default
            #line hidden
            this.Write("]\r\n\t\t([Deleted]\r\n\t\t,[CreatedBy_FK]\r\n\t\t,[Created]\r\n\t\t,[ModifiedBy_FK]\r\n\t\t,[Modifie" +
                    "d]\r\n\t\t,[");
            
            #line 29 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(relation.DbColumn1_Name));
            
            #line default
            #line hidden
            this.Write("]\r\n\t\t");
            
            #line 30 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
PushIndent("			");
		foreach(var col in columns){
			WriteLine(",[" + col.DbColumn_Name + "]");
		}
		PopIndent();
            
            #line default
            #line hidden
            this.Write("\t\t)\r\n\tSELECT\r\n\t\t0\r\n\t\t,@userId\r\n\t\t,getdate()\r\n\t\t,@userId\r\n\t\t,getdate()\r\n\t\t,@");
            
            #line 42 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(parentIdVarName));
            
            #line default
            #line hidden
            this.Write("\r\n\t\t");
            
            #line 43 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
PushIndent("			");
		foreach(var col in columns){
			WriteSelectColumn(col);
		}
		PopIndent();
            
            #line default
            #line hidden
            this.Write("\tFROM OPENJSON(@json");
            
            #line 48 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("Rows)\r\n\tWITH (\t\r\n\t\t[Deleted] [bit]\r\n\t\t");
            
            #line 51 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
PushIndent("			");
		foreach(var col in columns){
			WriteJsonColumn(col);
		}
		PopIndent();
            
            #line default
            #line hidden
            this.Write("\t\t) AS json");
            
            #line 56 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\n\tWHERE isnull(json");
            
            #line 57 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[Deleted], 0) = 0 AND json");
            
            #line 57 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[QueueOrder] = @rowQueueOrder");
            
            #line 57 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\n\r\n\tSELECT @newId");
            
            #line 59 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" = SCOPE_IDENTITY()\r\n\t---");
            
            #line 60 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(relation.DbObject1_Name));
            
            #line default
            #line hidden
            this.Write(" insert\r\n\r\n\t");
            
            #line 62 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
 foreach(var subItem in subItems){ 
            
            #line default
            #line hidden
            this.Write("\tdeclare @json");
            
            #line 63 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subItem.ProjectionRelation_ChildProjectionName));
            
            #line default
            #line hidden
            this.Write("Rows nvarchar(max) =   \r\n\t(SELECT TOP 1 json");
            
            #line 64 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[");
            
            #line 64 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subItem.ProjectionRelation_ChildProjectionName));
            
            #line default
            #line hidden
            this.Write("]\r\n\tFROM OPENJSON(@json");
            
            #line 65 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("Rows)\r\n\tWITH (\t\r\n\t\t[Deleted] [bit]\r\n\t\t,[QueueOrder] int\r\n\t\t,[");
            
            #line 69 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subItem.ProjectionRelation_ChildProjectionName));
            
            #line default
            #line hidden
            this.Write("] nvarchar(max) N\'$.");
            
            #line 69 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(subItem.ProjectionRelation_ChildProjectionName));
            
            #line default
            #line hidden
            this.Write("\' AS JSON\r\n\t\t) AS json");
            
            #line 70 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\n\tWHERE isnull(json");
            
            #line 71 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[Deleted], 0) = 0 AND json");
            
            #line 71 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(".[QueueOrder] = @rowQueueOrder");
            
            #line 71 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\n\t)\r\n\t");
            
            #line 73 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
WriteSubItem(subItem);
	}
            
            #line default
            #line hidden
            this.Write("\t\t\r\n\tfetch next from cursor");
            
            #line 76 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write(" into @rowQueueOrder");
            
            #line 76 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\nend\r\nclose cursor");
            
            #line 78 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\r\ndeallocate cursor");
            
            #line 79 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\Insert2_ItemGridInsert.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(childProjectionName));
            
            #line default
            #line hidden
            this.Write("\t\r\n");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
    #region Base class
    /// <summary>
    /// Base class for this transformation
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class Insert2_ItemGridInsertBase
    {
        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion
        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0) 
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField  = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField ;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField  = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }
    #endregion
}
