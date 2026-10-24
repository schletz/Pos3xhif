using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace RpgParty.Application
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            {
                Console.WriteLine("Checking the weapons of a character (List)");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    return HasNoSetter(typeof(Character), nameof(Character.Weapons))
                        && typeof(Character).PropertyHasType<IReadOnlyList<Weapon>>(nameof(Character.Weapons))
                        && hero.Weapons is not List<Weapon> && hero.Weapons.Count == 0;
                }, "Character.Weapons is a read-only IReadOnlyList<Weapon> and uses AsReadOnly().");
                CheckAndWrite(() =>
                    HasPrivateSetter(typeof(Character), nameof(Character.Weapon)) && CreateHero().Weapon is null,
                    "Character.Weapon cannot be set from outside the class.");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    hero.PickUp(Dagger);
                    var weaponAfterFirstPickUp = hero.Weapon;
                    hero.PickUp(Sword);
                    return weaponAfterFirstPickUp == Dagger && hero.Weapon == Dagger
                        && hero.Weapons.Count == 2 && hero.Weapons[0] == Dagger && hero.Weapons[1] == Sword;
                }, "PickUp adds the weapon and equips the first weapon.");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    hero.PickUp(Dagger);
                    hero.PickUp(Sword);
                    hero.Equip(Sword);
                    return hero.Weapon == Sword && hero.AttackPower == 25;
                }, "Equip equips a weapon of the inventory.");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    hero.PickUp(Sword);
                    try
                    {
                        hero.Equip(Axe);
                        return false;
                    }
                    catch (ArgumentException)
                    {
                        return hero.Weapon == Sword;
                    }
                }, "Equip throws an ArgumentException if the weapon is not in the inventory.");
                CheckAndWrite(() =>
                {
                    var farmer = new Character(name: "Bauer", maxHealth: 20, strength: 2);
                    farmer.EquipStrongestWeapon();
                    var hero = CreateHero();
                    hero.PickUp(Dagger);
                    hero.PickUp(Axe);
                    hero.PickUp(Sword);
                    hero.EquipStrongestWeapon();
                    return farmer.Weapon is null && hero.Weapon == Axe && hero.AttackPower == 35;
                }, "EquipStrongestWeapon equips the weapon with the highest damage.");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    hero.PickUp(Dagger);
                    hero.PickUp(Sword);
                    hero.PickUp(Axe);
                    hero.Equip(Axe);
                    bool firstDrop = hero.Drop(Axe);
                    bool secondDrop = hero.Drop(Axe);
                    return firstDrop && !secondDrop && hero.Weapon is null && hero.Weapons.Count == 2;
                }, "Drop removes the weapon and unequips it.");
            }

            {
                Console.WriteLine("Checking the skills of a character (HashSet)");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    return HasNoSetter(typeof(Character), nameof(Character.Skills))
                        && typeof(Character).PropertyHasType<IReadOnlySet<string>>(nameof(Character.Skills))
                        && hero.Skills is not HashSet<string> && hero.Skills.Count == 0;
                }, "Character.Skills is a read-only IReadOnlySet<string> and uses AsReadOnly().");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    bool learnedFireball = hero.LearnSkill("Feuerball");
                    bool learnedFireballAgain = hero.LearnSkill("Feuerball");
                    bool learnedHealing = hero.LearnSkill("Heilen");
                    return learnedFireball && !learnedFireballAgain && learnedHealing && hero.Skills.Count == 2;
                }, "LearnSkill ignores a skill the character already has.");
                CheckAndWrite(() =>
                {
                    var hero = CreateHero();
                    hero.LearnSkill("Heilen");
                    return hero.HasSkill("Heilen") && !hero.HasSkill("Teleport");
                }, "HasSkill returns true if the character has the skill.");
            }

            {
                Console.WriteLine("Checking Party (Dictionary)");
                CheckAndWrite(() => !typeof(Party).HasDefaultConstructor(), "Party does not have a default constructor.");
                CheckAndWrite(() =>
                {
                    var party = new Party(name: "Die Gefährten");
                    return HasNoSetter(typeof(Party), nameof(Party.Members))
                        && typeof(Party).PropertyHasType<IReadOnlyDictionary<string, Character>>(nameof(Party.Members))
                        && HasNoSetter(typeof(Party), nameof(Party.Supplies))
                        && typeof(Party).PropertyHasType<IReadOnlyDictionary<string, int>>(nameof(Party.Supplies))
                        && party.Members is not Dictionary<string, Character> && party.Supplies is not Dictionary<string, int>
                        && HasNoSetter(typeof(Party), nameof(Party.Name))
                        && party.Name == "Die Gefährten" && party.Members.Count == 0;
                }, "Party.Members and Party.Supplies are read-only IReadOnlyDictionary properties.");
                CheckAndWrite(() =>
                {
                    var party = new Party(name: "Die Gefährten");
                    var hero = CreateHero();
                    bool heroAdded = party.AddMember(hero);
                    bool mageAdded = party.AddMember(CreateMage());
                    return heroAdded && mageAdded && party.Members.Count == 2 && party.Members["Link"] == hero;
                }, "AddMember adds the character with the name as key.");
                CheckAndWrite(() =>
                {
                    var party = new Party(name: "Die Gefährten");
                    var hero = CreateHero();
                    party.AddMember(hero);
                    return !party.AddMember(new Character(name: "Link", maxHealth: 10, strength: 1))
                        && party.Members["Link"] == hero;
                }, "AddMember rejects a second member with the same name.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    return !party.AddMember(new Character(name: "Epona", maxHealth: 10, strength: 1))
                        && party.Members.Count == 4;
                }, "A party has at most 4 members.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    return party.FindMember("Zelda")?.Name == "Zelda" && party.FindMember("Ganon") is null;
                }, "FindMember returns the member or null.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    bool firstRemove = party.RemoveMember("Mercy");
                    bool secondRemove = party.RemoveMember("Mercy");
                    return firstRemove && !secondRemove && party.Members.Count == 3 && party.FindMember("Mercy") is null;
                }, "RemoveMember removes the member with this name.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    party.Members["Robin"].TakeDamage(1000);
                    List<Character> aliveMembers = party.AliveMembers;
                    return HasNoSetter(typeof(Party), nameof(Party.AliveMembers))
                        && aliveMembers.Count == 3 && !aliveMembers.Contains(party.Members["Robin"]);
                }, "AliveMembers returns the members who are alive.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    party.Members["Link"].LearnSkill("Feuerball");
                    party.Members["Link"].LearnSkill("Heilen");
                    party.Members["Zelda"].LearnSkill("Feuerball");
                    party.Members["Zelda"].LearnSkill("Teleport");
                    HashSet<string> partySkills = party.Skills;
                    return HasNoSetter(typeof(Party), nameof(Party.Skills))
                        && partySkills.Count == 3 && partySkills.Contains("Heilen") && partySkills.Contains("Teleport");
                }, "Party.Skills returns every skill of the members once.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    party.AddSupply("Heiltrank", 3);
                    party.AddSupply("Heiltrank", 2);
                    party.AddSupply("Fackel", 1);
                    return party.GetSupplyCount("Heiltrank") == 5 && party.GetSupplyCount("Fackel") == 1
                        && party.GetSupplyCount("Seil") == 0 && party.Supplies.Count == 2;
                }, "AddSupply and GetSupplyCount.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    try
                    {
                        party.AddSupply("Seil", 0);
                        return false;
                    }
                    catch (ArgumentException)
                    {
                        return !party.Supplies.ContainsKey("Seil");
                    }
                }, "AddSupply throws an ArgumentException if count is not greater than 0.");
                CheckAndWrite(() =>
                {
                    var party = CreateParty();
                    party.AddSupply("Heiltrank", 5);
                    party.AddSupply("Fackel", 1);
                    bool usedTorch = party.UseSupply("Fackel");
                    bool usedTorchAgain = party.UseSupply("Fackel");
                    bool usedPotion = party.UseSupply("Heiltrank");
                    return usedTorch && !usedTorchAgain && usedPotion
                        && !party.Supplies.ContainsKey("Fackel") && party.GetSupplyCount("Heiltrank") == 4;
                }, "UseSupply removes items that are used up.");
                CheckAndWrite(() =>
                {
                    var party = new Party(name: "Die Gefährten");
                    var hero = CreateHero();
                    var mage = CreateMage();
                    var archer = new Character(name: "Robin", maxHealth: 80, strength: 30);
                    party.AddMember(hero);
                    party.AddMember(mage);
                    party.AddMember(archer);
                    archer.TakeDamage(1000);
                    hero.PickUp(Sword);
                    // Link (10 + 15) and Zelda (20) hit the dragon (50 -> 5). Robin is dead and must not attack.
                    var dragon = new Character(name: "Drache", maxHealth: 50, strength: 40);
                    party.AttackTogether(dragon);
                    int dragonHealthAfterFirstAttack = dragon.Health;
                    party.AttackTogether(dragon);
                    // Only the member who kills the dragon gets the 50 experience points.
                    return dragonHealthAfterFirstAttack == 5 && !dragon.IsAlive
                        && hero.Experience + mage.Experience == 50 && archer.Experience == 0;
                }, "AttackTogether: all members who are alive attack the enemy.", weight: 2);
            }
            WriteSummary();
        }

        private static Weapon Dagger { get; } = new Weapon(name: "Dolch", damage: 5);
        private static Weapon Sword { get; } = new Weapon(name: "Schwert", damage: 15);
        private static Weapon Axe { get; } = new Weapon(name: "Axt", damage: 25);

        private static Character CreateHero() => new Character(name: "Link", maxHealth: 100, strength: 10);
        private static Character CreateMage() => new Character(name: "Zelda", maxHealth: 60, strength: 20);

        /// <summary>
        /// Creates a party with the 4 members Link, Zelda, Robin and Mercy.
        /// </summary>
        private static Party CreateParty()
        {
            var party = new Party(name: "Die Gefährten");
            party.AddMember(CreateHero());
            party.AddMember(CreateMage());
            party.AddMember(new Character(name: "Robin", maxHealth: 80, strength: 30));
            party.AddMember(new Character(name: "Mercy", maxHealth: 50, strength: 5));
            return party;
        }

        /// <summary>
        /// Returns true if the property exists and has no set method at all.
        /// </summary>
        private static bool HasNoSetter(Type type, string propertyName) =>
            type.GetProperty(propertyName) is { } property && !property.CanWrite;

        /// <summary>
        /// Returns true if the property has a set method that cannot be called from outside the class.
        /// </summary>
        private static bool HasPrivateSetter(Type type, string propertyName) =>
            type.GetProperty(propertyName)?.SetMethod is { } setMethod && !setMethod.IsPublic;
    }
}
