#pragma checksum "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "5cbf8ac24cd97b9aac7adef387c308913d621f59"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(LSystemsWebApp.Pages.Editor.Pages_Editor_Rules), @"mvc.1.0.razor-page", @"/Pages/Editor/Rules.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure.RazorPageAttribute(@"/Pages/Editor/Rules.cshtml", typeof(LSystemsWebApp.Pages.Editor.Pages_Editor_Rules), null)]
namespace LSystemsWebApp.Pages.Editor
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/_ViewImports.cshtml"
using LSystemsWebApp;

#line default
#line hidden
#line 2 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
using System.Text;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"5cbf8ac24cd97b9aac7adef387c308913d621f59", @"/Pages/Editor/Rules.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"0ab56a7c692f167f70e5de43e6d83708ab693cba", @"/Pages/_ViewImports.cshtml")]
    public class Pages_Editor_Rules : global::Microsoft.AspNetCore.Mvc.RazorPages.Page
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("flat-button"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "RemoveRule", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("method", "post", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_3 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("type", new global::Microsoft.AspNetCore.Html.HtmlString("submit"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_4 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "CreateRule", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_5 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("submit-button"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_6 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-page-handler", "NewRule", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_7 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("id", new global::Microsoft.AspNetCore.Html.HtmlString("create-form"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            BeginContext(66, 1, true);
            WriteLiteral("\n");
            EndContext();
#line 5 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
  
    Layout = "Shared/_EditorLayout";
    ViewData["MenuImage"] = "/svg/rule.svg";

#line default
#line hidden
            BeginContext(154, 1, true);
            WriteLiteral("\n");
            EndContext();
#line 10 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
 if (Model.ErrorMsg != null)
{

#line default
#line hidden
            BeginContext(186, 37, true);
            WriteLiteral("    <div class=\"card error\">\n        ");
            EndContext();
            BeginContext(224, 14, false);
#line 13 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
   Write(Model.ErrorMsg);

#line default
#line hidden
            EndContext();
            BeginContext(238, 13, true);
            WriteLiteral(";\n    </div>\n");
            EndContext();
#line 15 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
}

#line default
#line hidden
            BeginContext(253, 86, true);
            WriteLiteral("\n<div class=\"card\">\n    <h1>Rules</h1>\n    <h2>Define rules used in LSystem</h2>\n    \n");
            EndContext();
#line 21 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
     if (Model.RuleInfos.Count == 0)
    {

#line default
#line hidden
            BeginContext(382, 71, true);
            WriteLiteral("        <p>No rule was defined, start by clicking on button below.</p>\n");
            EndContext();
#line 24 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
    }
    else
    {

#line default
#line hidden
            BeginContext(474, 281, true);
            WriteLiteral(@"        <table>
            <tr>
                <th>Left context</th>
                <th>Source module</th>
                <th>Right context</th>
                <th>Parametric conditions</th>
                <th>Next generation</th>
                <th></th>
            </tr>
");
            EndContext();
#line 36 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
             for(int i = 0; i < Model.RuleInfos.Count; i++)
            {
                var info = Model.RuleInfos[i];

#line default
#line hidden
            BeginContext(876, 70, true);
            WriteLiteral("                <tr>\n                    <td>\n                        ");
            EndContext();
            BeginContext(947, 47, false);
#line 41 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                   Write(Model.BuildContextDescription(info.LeftContext));

#line default
#line hidden
            EndContext();
            BeginContext(994, 76, true);
            WriteLiteral("\n                    </td>\n                    <td>\n                        ");
            EndContext();
            BeginContext(1071, 17, false);
#line 44 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                   Write(info.SourceModule);

#line default
#line hidden
            EndContext();
            BeginContext(1088, 76, true);
            WriteLiteral("\n                    </td>\n                    <td>\n                        ");
            EndContext();
            BeginContext(1165, 48, false);
#line 47 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                   Write(Model.BuildContextDescription(info.RightContext));

#line default
#line hidden
            EndContext();
            BeginContext(1213, 27, true);
            WriteLiteral("\n                    </td>\n");
            EndContext();
#line 49 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                     if (info.ParametricConditions != null)
                    {

#line default
#line hidden
            BeginContext(1322, 28, true);
            WriteLiteral("                        <td>");
            EndContext();
            BeginContext(1351, 36, false);
#line 51 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                       Write(info.ParametricConditions.ToString());

#line default
#line hidden
            EndContext();
            BeginContext(1387, 6, true);
            WriteLiteral("</td>\n");
            EndContext();
#line 52 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                    }
                    else
                    {

#line default
#line hidden
            BeginContext(1462, 34, true);
            WriteLiteral("                        <td></td>\n");
            EndContext();
#line 56 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                    }

#line default
#line hidden
            BeginContext(1518, 49, true);
            WriteLiteral("                    <td>\n                        ");
            EndContext();
            BeginContext(1568, 50, false);
#line 58 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                   Write(Model.BuildNextGenDescription(info.NextGeneration));

#line default
#line hidden
            EndContext();
            BeginContext(1618, 76, true);
            WriteLiteral("\n                    </td>\n                    <td>\n                        ");
            EndContext();
            BeginContext(1694, 342, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5911517", async() => {
                BeginContext(1714, 67, true);
                WriteLiteral("\n                            <input name=\"rule-index\" type=\"number\"");
                EndContext();
                BeginWriteAttribute("value", " value=\"", 1781, "\"", 1791, 1);
#line 62 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
WriteAttributeValue("", 1789, i, 1789, 2, false);

#line default
#line hidden
                EndWriteAttribute();
                BeginContext(1792, 38, true);
                WriteLiteral(" hidden/>\n                            ");
                EndContext();
                BeginContext(1830, 174, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5912392", async() => {
                    BeginContext(1888, 107, true);
                    WriteLiteral("\n                                <img src=\"/svg/clear.svg\" alt=\"Remove rule\"/>\n                            ");
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_1.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2004, 25, true);
                WriteLiteral("\n                        ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(2036, 49, true);
            WriteLiteral("\n                    </td>\n                </tr>\n");
            EndContext();
#line 69 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
            }

#line default
#line hidden
            BeginContext(2099, 17, true);
            WriteLiteral("        </table>\n");
            EndContext();
#line 71 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
    }

#line default
#line hidden
            BeginContext(2122, 7, true);
            WriteLiteral("</div>\n");
            EndContext();
            BeginContext(2129, 107, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5915832", async() => {
                BeginContext(2149, 5, true);
                WriteLiteral("\n    ");
                EndContext();
                BeginContext(2154, 74, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("button", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5916213", async() => {
                    BeginContext(2206, 13, true);
                    WriteLiteral("Add new rule\"");
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_4.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_4);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2228, 1, true);
                WriteLiteral("\n");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(2236, 2, true);
            WriteLiteral("\n\n");
            EndContext();
#line 77 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
 if (Model.DisplayEditor)
{

#line default
#line hidden
            BeginContext(2266, 57, true);
            WriteLiteral("    <div class=\"card\">\n        <h1>New rule</h1>\n        ");
            EndContext();
            BeginContext(2323, 3369, false);
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5919316", async() => {
                BeginContext(2360, 120, true);
                WriteLiteral("\n            <span class=\"form-desc\">Source module:</span>\n            <select class=\"form-field\" name=\"source-module\">\n");
                EndContext();
#line 84 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                 foreach (var module in Model.ModuleInfos)
                {

#line default
#line hidden
                BeginContext(2557, 20, true);
                WriteLiteral("                    ");
                EndContext();
                BeginContext(2577, 35, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "5cbf8ac24cd97b9aac7adef387c308913d621f5920139", async() => {
                    BeginContext(2586, 17, false);
#line 86 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                       Write(module.ModuleName);

#line default
#line hidden
                    EndContext();
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(2612, 1, true);
                WriteLiteral("\n");
                EndContext();
#line 87 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                }

#line default
#line hidden
                BeginContext(2631, 209, true);
                WriteLiteral("            </select>\n            <br/>\n            <span class=\"form-desc\">Next generation:</span>\n            <div id=\"editor_rules_modules-selection-next\"\n                 class=\"modules-selector-options\">\n");
                EndContext();
#line 93 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                 foreach (var module in Model.ModuleInfos)
                {

#line default
#line hidden
                BeginContext(2917, 106, true);
                WriteLiteral("                    <div class=\"editor_axiom_modules-module\">\n                        <span class=\"title\">");
                EndContext();
                BeginContext(3024, 17, false);
#line 96 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                                       Write(module.ModuleName);

#line default
#line hidden
                EndContext();
                BeginContext(3041, 8, true);
                WriteLiteral("</span>\n");
                EndContext();
#line 97 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                         foreach (var param in module.ParamNames)
                        {

#line default
#line hidden
                BeginContext(3141, 93, true);
                WriteLiteral("                            <div class=\"module-param\">\n                                <span>");
                EndContext();
                BeginContext(3235, 5, false);
#line 100 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                                 Write(param);

#line default
#line hidden
                EndContext();
                BeginContext(3240, 105, true);
                WriteLiteral("</span>\n                                <input type=\"text\" disabled/>\n                            </div>\n");
                EndContext();
#line 103 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                        }

#line default
#line hidden
                BeginContext(3371, 27, true);
                WriteLiteral("                    </div>\n");
                EndContext();
#line 105 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                }

#line default
#line hidden
                BeginContext(3416, 644, true);
                WriteLiteral(@"            </div>
            <div id=""editor_rules_modules-selected-next"" 
                 class=""modules-selector-selected"">
                    
            </div>
            <div id=""editor_rules_modules-bin-next"" 
                 class=""modules-selector-bin"">
                    
            </div>
            <button id=""show-advanced"" type=""button"">Show advanced options</button>
            <br/>
            <div id=""advanced"" style=""display: none;"">
                <span class=""form-desc"">Left context:</span>
                <div id=""editor_rules_modules-selection-left""
                     class=""modules-selector-options"">
");
                EndContext();
#line 121 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                     foreach (var module in Model.ModuleInfos)
                    {

#line default
#line hidden
                BeginContext(4145, 114, true);
                WriteLiteral("                        <div class=\"editor_axiom_modules-module\">\n                            <span class=\"title\">");
                EndContext();
                BeginContext(4260, 17, false);
#line 124 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                                           Write(module.ModuleName);

#line default
#line hidden
                EndContext();
                BeginContext(4277, 39, true);
                WriteLiteral("</span>\n                        </div>\n");
                EndContext();
#line 126 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                    }

#line default
#line hidden
                BeginContext(4338, 525, true);
                WriteLiteral(@"                </div>
                <div id=""editor_rules_modules-selected-left"" 
                     class=""modules-selector-selected"">
                        
                </div>
                <div id=""editor_rules_modules-bin-left"" 
                     class=""modules-selector-bin"">
                        
                </div>
                <span class=""form-desc"">Right context:</span>
                <div id=""editor_rules_modules-selection-right""
                     class=""modules-selector-options"">
");
                EndContext();
#line 139 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                     foreach (var module in Model.ModuleInfos)
                    {

#line default
#line hidden
                BeginContext(4948, 114, true);
                WriteLiteral("                        <div class=\"editor_axiom_modules-module\">\n                            <span class=\"title\">");
                EndContext();
                BeginContext(5063, 17, false);
#line 142 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                                           Write(module.ModuleName);

#line default
#line hidden
                EndContext();
                BeginContext(5080, 39, true);
                WriteLiteral("</span>\n                        </div>\n");
                EndContext();
#line 144 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
                    }

#line default
#line hidden
                BeginContext(5141, 460, true);
                WriteLiteral(@"                </div>
                <div id=""editor_rules_modules-selected-right"" 
                     class=""modules-selector-selected"">
                        
                </div>
                <div id=""editor_rules_modules-bin-right"" 
                     class=""modules-selector-bin"">
                        
                </div>
            </div>
            
            <input id=""save-button"" type=""button"" value=""Add rule""/>
            ");
                EndContext();
                BeginContext(5601, 75, false);
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("input", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.SelfClosing, "5cbf8ac24cd97b9aac7adef387c308913d621f5927944", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormActionTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_5);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_3);
                __Microsoft_AspNetCore_Mvc_TagHelpers_FormActionTagHelper.PageHandler = (string)__tagHelperAttribute_6.Value;
                __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_6);
                BeginWriteTagHelperAttribute();
                __tagHelperStringValueBuffer = EndWriteTagHelperAttribute();
                __tagHelperExecutionContext.AddHtmlAttribute("hidden", Html.Raw(__tagHelperStringValueBuffer), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.Minimized);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                EndContext();
                BeginContext(5676, 9, true);
                WriteLiteral("\n        ");
                EndContext();
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_7);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Method = (string)__tagHelperAttribute_2.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            EndContext();
            BeginContext(5692, 12, true);
            WriteLiteral("\n    </div>\n");
            EndContext();
#line 160 "/home/jinsi/Projects/LSystems/LSystemsWebApp/Pages/Editor/Rules.cshtml"
}

#line default
#line hidden
            BeginContext(5706, 2236, true);
            WriteLiteral(@"
<script src=""https://raw.githack.com/SortableJS/Sortable/master/Sortable.js""></script>
<script src=""/js/drag_form_helpers.js""></script>
<script>
    var advanced_toggle = document.getElementById('show-advanced');
    advanced_toggle.addEventListener('click', function() {toogle_advanced(advanced_toggle)});

    var form = document.getElementById('create-form');
    var save_btn = document.getElementById('save-button');
    var submit_btn = document.getElementById('submit-button');
    
    //Next Generation
    var source_list_next = document.getElementById(""editor_rules_modules-selection-next"");
    var target_list_next = document.getElementById(""editor_rules_modules-selected-next"");
    var bin_next = document.getElementById(""editor_rules_modules-bin-next"");
    //Left Context
    var source_list_left = document.getElementById(""editor_rules_modules-selection-left"");
    var target_list_left = document.getElementById(""editor_rules_modules-selected-left"");
    var bin_left = document.getElementById(""editor_ru");
            WriteLiteral(@"les_modules-bin-left"");
    //Right Context
    var source_list_right = document.getElementById(""editor_rules_modules-selection-right"");
    var target_list_right = document.getElementById(""editor_rules_modules-selected-right"");
    var bin_right = document.getElementById(""editor_rules_modules-bin-right"");
    
    InitDragForm(source_list_next, target_list_next, bin_next, 'next');
    InitDragForm(source_list_left, target_list_left, bin_left, 'left');
    InitDragForm(source_list_right, target_list_right, bin_right, 'right');
    
    save_btn.addEventListener('click', function() {
       SubmitData(target_list_next, form, 'next');
       SubmitData(target_list_left, form, 'left');
       SubmitData(target_list_right, form, 'right');
       submit_btn.click();
    });
    
    function toogle_advanced(button) {
        console.log(""toggling"");
        var advanced = document.getElementById('advanced');
        if(advanced.style.display == 'none') {
            advanced.style.display = 'block';
            bu");
            WriteLiteral("tton.innerHTML = \'Hide advanced options\';\n        } else {\n            advanced.style.display = \'none\';\n            button.innerHTML = \'Show advanced options\'\n        }\n    }    \n</script>");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<LSystemsWebApp.Pages.Editor.Rules> Html { get; private set; }
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<LSystemsWebApp.Pages.Editor.Rules> ViewData => (global::Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary<LSystemsWebApp.Pages.Editor.Rules>)PageContext?.ViewData;
        public LSystemsWebApp.Pages.Editor.Rules Model => ViewData.Model;
    }
}
#pragma warning restore 1591
