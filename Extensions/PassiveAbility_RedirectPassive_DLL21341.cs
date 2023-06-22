using LOR_DiceSystem;

namespace UtilLoader21341.Extensions
{
    public class PassiveAbility_RedirectDiePassive_DLL21341 : PassiveAbilityBase
    {
        private BehaviourDetail _detailType = BehaviourDetail.None;
        public int SpeedDieSlot = -1;

        public void SetDetailType(BehaviourDetail detailType)
        {
            _detailType = detailType;
        }

        public override void OnCreated()
        {
            Hide();
        }

        public override void OnAfterRollSpeedDice()
        {
            SetRedirectSpeedDie();
        }

        public bool DiceMatch(DiceMatch x)
        {
            return _detailType == BehaviourDetail.None || x.abiliity.behaviourInCard.Detail == _detailType;
        }

        public override void OnUseCard(BattlePlayingCardDataInUnitModel curCard)
        {
            if (SpeedDieSlot == curCard.slotOrder)
                curCard.ApplyDiceStatBonus(DiceMatch, new DiceStatBonus
                {
                    power = 1
                });
        }

        public override void OnStartTargetedOneSide(BattlePlayingCardDataInUnitModel attackerCard)
        {
            if (SpeedDieSlot == attackerCard.targetSlotOrder)
                attackerCard.ApplyDiceStatBonus(DiceMatch, new DiceStatBonus
                {
                    min = -1,
                    max = -1
                });
        }

        public override void OnStartParrying(BattlePlayingCardDataInUnitModel card)
        {
            if (SpeedDieSlot != card.slotOrder) return;
            var target = card.target;
            var battlePlayingCardDataInUnitModel = target?.currentDiceAction;
            battlePlayingCardDataInUnitModel?.ApplyDiceStatBonus(DiceMatch, new DiceStatBonus
            {
                min = -1,
                max = -1
            });
        }

        private void SetRedirectSpeedDie(int? speedDieSlot = null)
        {
            SpeedDieSlot = speedDieSlot ?? (owner.speedDiceResult.Count - 2 < 0 ? 0 : owner.speedDiceResult.Count - 2);
        }
    }
}