﻿/*
 * SonarAnalyzer for .NET
 * Copyright (C) 2015-2018 SonarSource SA
 * mailto: contact AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using SonarAnalyzer.Helpers;

namespace SonarAnalyzer.Rules.Common
{
    public abstract class PropertyWriteOnlyBase : SonarDiagnosticAnalyzer
    {
        protected const string DiagnosticId = "S2376";
        protected const string MessageFormat = "Provide a getter for '{0}' or replace the property with a 'Set{0}' method.";

        protected abstract GeneratedCodeRecognizer GeneratedCodeRecognizer { get; }
    }

    public abstract class PropertyWriteOnlyBase<TLanguageKindEnum, TPropertyDeclaration> : PropertyWriteOnlyBase
        where TLanguageKindEnum : struct
        where TPropertyDeclaration : SyntaxNode
    {
        protected sealed override void Initialize(SonarAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                GeneratedCodeRecognizer,
                c =>
                {
                    var prop = (TPropertyDeclaration)c.Node;
                    if (!IsWriteOnlyProperty(prop))
                    {
                        return;
                    }

                    var identifier = GetIdentifier(prop);
                    c.ReportDiagnosticWhenActive(Diagnostic.Create(Rule, identifier.GetLocation(),
                        identifier.ValueText));
                },
                SyntaxKindsOfInterest.ToArray());
        }

        protected abstract SyntaxToken GetIdentifier(TPropertyDeclaration prop);

        protected abstract bool IsWriteOnlyProperty(TPropertyDeclaration prop);

        public abstract ImmutableArray<TLanguageKindEnum> SyntaxKindsOfInterest { get; }

        protected abstract DiagnosticDescriptor Rule { get; }

        public override sealed ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
    }
}
