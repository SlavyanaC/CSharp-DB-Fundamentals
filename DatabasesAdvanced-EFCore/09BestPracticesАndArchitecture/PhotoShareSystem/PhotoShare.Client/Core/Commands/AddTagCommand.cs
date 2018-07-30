namespace PhotoShare.Client.Core.Commands
{
    using System;

    using PhotoShare.Client.Core.Contracts;
    using PhotoShare.Client.Core.Exceptions;
    using PhotoShare.Client.Utilities;
    using PhotoShare.Services.Contracts;

    public class AddTagCommand : ICommand
    {
        private const string SUCCESSFULY_ADDED = "Tag {0} was added successfully!";
        private const string TAG_EXISTS = "Tag {0} exists!";

        private readonly IUserSessionService userSessionService;
        private readonly ITagService tagService;

        public AddTagCommand(IUserSessionService userSessionService, ITagService tagService)
        {
            this.userSessionService = userSessionService;
            this.tagService = tagService;
        }

        // AddTag <tag>
        public string Execute(string[] args)
        {
            string tagName = string.Join(" ", args);
            bool exists = this.tagService.Exists(tagName);

            bool isLogged = this.userSessionService.IsLoggedIn();

            if (!isLogged)
            {
                throw new InvalidCredentialsException();
            }

            if (exists)
            {
                throw new ArgumentException(string.Format(TAG_EXISTS, tagName));
            }

            tagName = tagName.ValidateOrTransform();

            this.tagService.AddTag(tagName);

            return string.Format(SUCCESSFULY_ADDED, tagName);
        }
    }
}
