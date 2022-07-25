namespace GolemAutomation
{
    class HarvestableWithIdea : Harvestable
    {
        public string[] IdeaDrops;

        public override void Emptied()
        {
            foreach (var idea in IdeaDrops)
            {
                if (!WorldManager.instance.HasFoundCard(idea))
                {
                    WorldManager.instance
                        .CreateCard(transform.position, idea, checkAddToStack: false)
                        .MyGameCard.SendIt();
                    return;
                }
            }
        }
    }
}
