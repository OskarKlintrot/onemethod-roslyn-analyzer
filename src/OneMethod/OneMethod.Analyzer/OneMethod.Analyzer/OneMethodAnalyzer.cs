using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace OneMethod.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class OneMethodAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "OM0001";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Desgin";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            context.RegisterSyntaxNodeAction(AnalyzeSyntax, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeSyntax(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is ClassDeclarationSyntax node
                && !node.Modifiers.Any(x => x.ValueText.Equals(SyntaxFactory.Token(SyntaxKind.StaticKeyword).ValueText)))
            {
                var keywords = new[] {
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword).ValueText,
                    SyntaxFactory.Token(SyntaxKind.InternalKeyword).ValueText,
                };

                var members = node.Members
                    .OfType<MethodDeclarationSyntax>()
                    .ToList();

                var limit = node.Identifier.ValueText.Equals("program", StringComparison.OrdinalIgnoreCase)
                    && members.Count(x => x.Identifier.ValueText.Equals("main", StringComparison.OrdinalIgnoreCase)) > 0
                    ? 1
                    : 2;

                var methodDeclarationSyntaxes = members
                    .Where(x => x.Modifiers.Any(y => keywords.Contains(y.ValueText)))
                    .ToList();

                if (methodDeclarationSyntaxes.Count >= limit)
                {
                    var diagnostic = Diagnostic.Create(
                       Rule,
                       node.Identifier.GetLocation(),
                       node.Identifier.ValueText);

                    context.ReportDiagnostic(diagnostic);

                    foreach (var item in methodDeclarationSyntaxes)
                    {
                        diagnostic = Diagnostic.Create(
                        Rule,
                        item.Identifier.GetLocation(),
                        node.Identifier.ValueText);

                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
