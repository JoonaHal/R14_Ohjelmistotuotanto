using CommunityToolkit.Mvvm.Input;
using Mökinvaraus.Models;

namespace Mökinvaraus.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}