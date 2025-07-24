using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Messaging;

namespace Lumo.Application.Stories.CreateNewDraft;
public sealed record CreateNewDraftCommand(
    string Title,
    string Content
) : ICommand<DraftDto>;
