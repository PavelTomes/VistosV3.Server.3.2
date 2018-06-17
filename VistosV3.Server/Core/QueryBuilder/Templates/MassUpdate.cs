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
    
    #line 1 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class MassUpdate : TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            this.Write("\r\n\tBEGIN TRY\r\n\t\tBEGIN TRANSACTION \r\n\t\t\tUPDATE [");
            
            #line 9 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(vwProjection.DbObject_Schema));
            
            #line default
            #line hidden
            this.Write("].[");
            
            #line 9 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(vwProjection.DbObject_Name));
            
            #line default
            #line hidden
            this.Write("]\r\n\t\t\tSET \r\n\t\t\t  [ModifiedBy_FK] = @userId\r\n\t\t\t,[Modified] = getdate()\r\n");
            
            #line 13 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"
 
			PushIndent("			");
			foreach(var col in columns){
				WriteUpdateColumn(col);
			}
			PopIndent();

            
            #line default
            #line hidden
            this.Write("\r\n\t\t\tFROM OPENJSON(@json)\r\n\t\t\t\tWITH (\r\n");
            
            #line 23 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"

			PushIndent("			");
			int columnCount= columns.Count();
			int columnIndex=0;
			foreach(var col in columns){
				WriteJsonColumn(col);
				columnIndex++;
				if(columnIndex!=columnCount){
					Write(",");
				}
			}
			PopIndent();

            
            #line default
            #line hidden
            this.Write("\t\t\t\t\t\r\n\t\t\t\t) AS json\r\n\t\t\t\t ");
            
            #line 37 "P:\WorkEswGitHub\VistosV3.Server.3.2a\VistosV3.Server\Core\QueryBuilder\Templates\MassUpdate.tt"
WriteUpdateIds(); 
            
            #line default
            #line hidden
            this.Write(@"				

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH 
		IF (@@TRANCOUNT > 0)
		   BEGIN
			  ROLLBACK TRANSACTION
		   END 
			
			--SELECT
			--	ERROR_NUMBER() AS ErrorNumber,
			--	ERROR_SEVERITY() AS ErrorSeverity,
			--	ERROR_STATE() AS ErrorState,
			--	ERROR_PROCEDURE() AS ErrorProcedure,
			--	ERROR_LINE() AS ErrorLine,
			--	ERROR_MESSAGE() AS ErrorMessage

				declare @ERROR_NUMBER int = 50000;
				declare @ERROR_MESSAGE nvarchar(2048) = (isnull(cast((ERROR_PROCEDURE()) as varchar) + '. ', '') + ERROR_MESSAGE());
				declare @ERROR_STATE int = ERROR_STATE();

				THROW @ERROR_NUMBER, @ERROR_MESSAGE, @ERROR_STATE;
	END CATCH
");
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}