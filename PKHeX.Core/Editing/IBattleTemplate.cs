namespace PKHeX.Core
{
    /// <summary>
    /// Interface containing details relevant for battling.
    /// </summary>
    public interface IBattleTemplate : ISpeciesForm, IGigantamax, INature
    {
        /// <summary>
        /// <see cref="PKM.Format"/> of the Set entity it is specific to.
        /// </summary>
        int Format { get; }

        /// <summary>
        /// <see cref="PKM.Nickname"/> of the Set entity.
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// <see cref="PKM.Gender"/> name of the Set entity.
        /// </summary>
        int Gender { get; }

        /// <summary>
        /// <see cref="PKM.HeldItem"/> of the Set entity.
        /// </summary>
        int HeldItem { get; }

        /// <summary>
        /// <see cref="PKM.Ability"/> of the Set entity.
        /// </summary>
        int Ability { get; }

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        int Level { get; }

        /// <summary>
        /// <see cref="PKM.CurrentLevel"/> of the Set entity.
        /// </summary>
        bool Shiny { get; }

        /// <summary>
        /// <see cref="PKM.CurrentFriendship"/> of the Set entity.
        /// </summary>
        int Friendship { get; }

        /// <summary>
        /// <see cref="PKM.Form"/> name of the Set entity, stored in PKHeX style (instead of Showdown's)
        /// </summary>
        string FormName { get; }

        /// <summary>
        /// <see cref="PKM.HPType"/> of the Set entity.
        /// </summary>
        int HiddenPowerType { get; }

        /// <summary>
        /// <see cref="PKM.EVs"/> of the Set entity.
        /// </summary>
        int[] EVs { get; }

        /// <summary>
        /// <see cref="PKM.IVs"/> of the Set entity.
        /// </summary>
        int[] IVs { get; }

        /// <summary>
        /// <see cref="PKM.Moves"/> of the Set entity.
        /// </summary>
        int[] Moves { get; }
    }
}
