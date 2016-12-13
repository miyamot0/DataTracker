namespace DataTracker.Model
{
    class Group
    {
        public static Group CreateGroup(string groupName)
        {
            return new Group { GroupName = groupName };
        }

        public string GroupName { get; set; }
    }
}
