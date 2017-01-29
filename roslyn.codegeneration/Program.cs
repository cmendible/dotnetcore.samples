using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Roslyn.CodeGeneration
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // We will change the namespace of this sample code.
            var code =
            @"  using System; 

                namespace OldNamespace 
                { 
                    public class Person
                    {
                        public string Name { get; set; }
                        public int Age {get; set; }
                    }
                }";

            // Use Task to call ChangeNamespaceAsync
            Task.Run(async () =>
            {
                await ChangeNamespaceAsync(code, "NamespaceChangedUsingRoslyn");
            })
            .GetAwaiter()
            .GetResult();

            // Wait to exit.
            Console.Read();
        }

        /// <summary>
        /// Changes the namespace for the given code.
        /// </summary>
        static async Task ChangeNamespaceAsync(string code, string @namespace)
        {
            // Parse the code into a SyntaxTree.
            var tree = CSharpSyntaxTree.ParseText(code);

            // Get the root CompilationUnitSyntax.
            var root = await tree.GetRootAsync().ConfigureAwait(false) as CompilationUnitSyntax;

            // Get the namespace declaration.
            var oldNamespace = root.Members.Single(m => m is NamespaceDeclarationSyntax) as NamespaceDeclarationSyntax;

            // Get all class declarations inside the namespace.
            var classDeclarations = oldNamespace.Members.Where(m => m is ClassDeclarationSyntax);

            // Create a new namespace declaration.
            var newNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(@namespace)).NormalizeWhitespace();

            // Add the class declarations to the new namespace.
            newNamespace = newNamespace.AddMembers(classDeclarations.Cast<MemberDeclarationSyntax>().ToArray());

            // Replace the oldNamespace with the newNamespace and normailize.
            root = root.ReplaceNode(oldNamespace, newNamespace).NormalizeWhitespace();

            string newCode = root.ToFullString();

            // Write the new file.
            File.WriteAllText("Person.cs", root.ToFullString());

            // Output new code to the console.
            Console.WriteLine(newCode);
            Console.WriteLine("Namespace replaced...");
        }
    }
}