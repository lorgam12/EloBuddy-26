using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using EloBuddy.SDK.Events;
using EloBuddy.SDK;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using EloBuddy.SDK.Utils;
using SharpDX;


namespace LBZero
{


    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }

        private static AIHeroClient LB = ObjectManager.Player;

        private static AIHeroClient User = Player.Instance;

        private static Spell.Targeted Q;

        private static Spell.Skillshot W;

        private static Spell.Skillshot E;

        private static Spell.Targeted R;

        private static Spell.Active WReturn;

        private static Spell.Active RReturn;

        private static Spell.Targeted QUlti;

        private static Spell.Skillshot WUlti;

        private static Spell.Skillshot EUlti;




        private static string[] Combos = { "Q -> R", "W -> R", "E -> R" };

        private static Item HealthPotion, HuntersPotion, Biscuit, CorruptPotion, RefillPotion;





        private static Menu LBMenu, ComboMenu, LaneClearMenu, FleeMenu, ItemMenu, DrawingsMenu, SkinChangerMenu;





        private static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();







        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (User.ChampionName != "Leblanc")
            {
                return;
            }

            Q = new Spell.Targeted(spellSlot: SpellSlot.Q, spellRange: 700);

            W = new Spell.Skillshot(spellSlot: SpellSlot.W, spellRange: 600, skillShotType: SkillShotType.Circular, castDelay: 250, spellSpeed: 1450, spellWidth: 250);

            E = new Spell.Skillshot(spellSlot: SpellSlot.E, spellRange: 950, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 55);
            { E.AllowedCollisionCount = 0; }
            R = new Spell.Targeted(spellSlot: SpellSlot.R, spellRange: 950);

            WReturn = new Spell.Active(SpellSlot.W);

            RReturn = new Spell.Active(SpellSlot.R);

            QUlti = new Spell.Targeted(SpellSlot.R, Q.Range);

            WUlti = new Spell.Skillshot(SpellSlot.R, W.Range, SkillShotType.Circular, 250, 1450, 250);

            EUlti = new Spell.Skillshot(spellSlot: SpellSlot.R, spellRange: 950, skillShotType: SkillShotType.Linear, castDelay: 250, spellSpeed: 1550, spellWidth: 55);
            { EUlti.AllowedCollisionCount = 0; }


            HealthPotion = new Item(2003, 0);
            Biscuit = new Item(2010, 0);
            CorruptPotion = new Item(2033, 0);
            RefillPotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);




            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);





            Chat.Print("LeBlancZero loaded successfully");



            CurrentSkinID = User.SkinId;




            





            LBMenu = MainMenu.AddMenu("LeBlancZero", "LeBlancZero");

            ComboMenu = LBMenu.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");
            ComboMenu.Add("Q", new CheckBox("Use Q"));
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("Wreturn", new CheckBox("Use W Return"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.Add("R", new CheckBox("Use R"));
            ComboMenu.AddSeparator(50);
            ComboMenu.AddGroupLabel("Ultimate Settings");
            ComboMenu.Add("Rcombo", new ComboBox("Combos",0, Combos));
            ComboMenu.Add("Rreturn", new CheckBox("Use R Return"));



            LaneClearMenu = LBMenu.AddSubMenu("LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("LCQ", new CheckBox("Use Q"));
            LaneClearMenu.Add("LCW", new CheckBox("Use W"));
            LaneClearMenu.Add("WE", new Slider("Use W when it hits Minions >=", 3, 1, 10));



            FleeMenu = LBMenu.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("FleeW", new CheckBox("Use W"));
            FleeMenu.Add("FleeR", new CheckBox("Use R"));

            ItemMenu = LBMenu.AddSubMenu("Items");
            ItemMenu.AddGroupLabel("Item Settings");
            ItemMenu.Add("potion", new CheckBox("Auto Use Potion"));
            ItemMenu.Add("potionhp", new Slider("%HP to use Potion <=", 70, 0, 100));


            DrawingsMenu = LBMenu.AddSubMenu("Drawings");
            DrawingsMenu.AddGroupLabel("Drawing Settings");
            DrawingsMenu.Add("DrawQ", new CheckBox("Draw Q"));
            DrawingsMenu.Add("DrawW", new CheckBox("Draw W"));
            DrawingsMenu.Add("DrawE", new CheckBox("Draw E"));




            SkinChangerMenu = LBMenu.AddSubMenu("Skin Changer");
            SkinChangerMenu.Add("EnableSkin", new CheckBox("Enable Skinchanger", false));
            SkinChangerMenu.Add("SkinID", new Slider("Skin ID", 1, 0, 5));





            Drawing.OnDraw += Drawing_OnDraw;






            


            Game.OnTick += Game_OnTick;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawingsMenu.Get<CheckBox>("DrawQ").CurrentValue && Q.IsLearned)
            {
                if (Q.IsReady())
                {
                    Circle.Draw(Color.White, Q.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawingsMenu.Get<CheckBox>("DrawQ").CurrentValue && Q.IsLearned)
            {
                if (!Q.IsReady())
                {
                    Circle.Draw(Color.Black, Q.Range, ObjectManager.Player.Position);
                }
            }



            if (DrawingsMenu.Get<CheckBox>("DrawW").CurrentValue && W.IsLearned)
            {
                if (W.IsReady())
                {
                    Circle.Draw(Color.Aqua, W.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawingsMenu.Get<CheckBox>("DrawW").CurrentValue && W.IsLearned)
            {
                if (!W.IsReady())
                {
                    Circle.Draw(Color.DarkSlateBlue, W.Range, ObjectManager.Player.Position);
                }
            }



            if (DrawingsMenu.Get<CheckBox>("DrawE").CurrentValue && E.IsLearned)
            {
                if (E.IsReady())
                {
                    Circle.Draw(Color.Red, E.Range, ObjectManager.Player.Position);
                }
            }

            if (DrawingsMenu.Get<CheckBox>("DrawE").CurrentValue && E.IsLearned)
            {
                if (!E.IsReady())
                {
                    Circle.Draw(Color.DarkRed, E.Range, ObjectManager.Player.Position);
                }
            }

        }






        private static void Game_OnTick(EventArgs args)
        {
            if (User.IsDead || MenuGUI.IsChatOpen || User.IsRecalling())
            {
                return;
            }

            //if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            //{
            //    Combo();
            //    UltCombos();  
            //}

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

           


            AutoPot();


            if (SkinChangerMenu["EnableSkin"].Cast<CheckBox>().CurrentValue)
            {
                if (SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue != Player.Instance.SkinId)
                {
                    Player.SetSkinId(SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue);
                }
            }
            else if (!SkinChangerMenu["EnableSkin"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId(CurrentSkinID);
                SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue = CurrentSkinID;
            }

            //if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            //{
            //    LaneClear();
            //}


        }




        private static int CurrentSkinID;

        private static void UltCombos()
        {
            switch (ComboMenu["Rcombos"].Cast<ComboBox>().SelectedIndex)
            {

                case 0:
                    QRCombo();
                    break;

                case 1:
                    WRCombo();
                    break;

                case 2:
                    ERCombo();
                    break;

            }


        }


        //private static void Combo()
        //{

        //    var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);



        //    if (ComboMenu["Rcombos"].Cast<ComboBox>().CurrentValue )
        //    {
        //        if (target == null)
        //        {
        //            return;
        //        }

        //        if (target.IsValidTarget() && Q.IsReady() && Q.IsInRange(target))
        //        {
        //            Q.Cast(target);
        //        }

        //    }




        //}


        private static void QRCombo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if(Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlideReturn" && !W.IsReady())
            {
                if(target == null)
                {
                    return;
                }

                if(ComboMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady() && E.IsInRange(target))
                {
                    E.Cast(target);
                }

            }
            else if(!Q.IsReady() && !QUlti.IsReady())
            {
                if(target == null)
                {
                    return;
                }
                if(ComboMenu["W"].Cast<CheckBox>().CurrentValue && W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide" && Qmarked(target))
                {
                    W.Cast(target);
                }
           
                        
            }

            else
            {
                if (target == null)
                {
                    return;
                }
                if (target.IsValidTarget() && Q.IsReady() && Q.IsInRange(target) && ComboMenu["Q"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(target);
                }

                if (target.IsValidTarget() && !Q.IsReady() && ComboMenu["R"].Cast<CheckBox>().CurrentValue && QUlti.IsReady() && QUlti.IsInRange(target) && Player.Instance.Spellbook.GetSpell(SpellSlot.R).Name == "LeblancChaosOrbm")
                {
                    QUlti.Cast(target);
                }
            }
        }


        private static void WRCombo()
        {
            
        }

        private static void ERCombo()
        {

        }








        private static void AutoPot()
        {
            if (ItemMenu["potion"].Cast<CheckBox>().CurrentValue && !Player.Instance.IsInShopRange() && Player.Instance.HealthPercent <= ItemMenu["potionhp"].Cast<Slider>().CurrentValue && !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemCrystalFlaskJungle") || Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
            {
                {
                    if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                    {
                        HealthPotion.Cast();
                        return;
                    }
                    if (Item.HasItem(CorruptPotion.Id) && Item.CanUseItem(CorruptPotion.Id))
                    {
                        CorruptPotion.Cast();
                        return;
                    }
                    if (Item.HasItem(Biscuit.Id) && Item.CanUseItem(Biscuit.Id))
                    {
                        Biscuit.Cast();
                        return;
                    }
                    if (Item.HasItem(RefillPotion.Id) && Item.CanUseItem(RefillPotion.Id))
                    {
                        RefillPotion.Cast();
                        return;
                    }
                    if (Item.HasItem(HuntersPotion.Id) && Item.CanUseItem(HuntersPotion.Id))
                    {
                        HuntersPotion.Cast();
                        return;
                    }
                }
            }



        }



        private static bool Qmarked(Obj_AI_Base target)
        {
            return target.HasBuff("LeblancMarkOfSilence") || target.HasBuff("LeblancMarkOfSilenceM");
        }


        private static bool Emarked(Obj_AI_Base target)
        {
            return target.HasBuff("LeblancShackleBeam") || target.HasBuff("LeblancShackleBeamM");
        }

        //private static void LaneClear()
        //{
        //    var UseE = LaneClearMenu["Elc"].Cast<Slider>().CurrentValue;


        //    if (LaneClearMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady())
        //    {
        //        var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
        //        var pred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(target, 225, (int)E.Range);


        //        if (pred.HitNumber >= UseE)
        //        {
        //            E.Cast(pred.CastPosition);
        //        }
        //    }


        //    if (ItemMenu["tialc"].Cast<CheckBox>().CurrentValue)
        //    {
        //        var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
        //        var pred1 = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(target, 400, 400);
        //        if (pred1.HitNumber >= UseE)
        //        {
        //            tiamat.Cast();
        //        }

        //    }

        //    if (ItemMenu["hydralc"].Cast<CheckBox>().CurrentValue)
        //    {
        //        var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
        //        var pred1 = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(target, 400, 400);
        //        if (pred1.HitNumber >= UseE)
        //        {
        //            hydra.Cast();
        //        }



        //    }

        //}





        private static void Flee()
        {
            if (FleeMenu["FleeW"].Cast<CheckBox>().CurrentValue && W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlide")
            {
                W.Cast(Player.Instance.ServerPosition.Extend(Game.CursorPos, W.Range).To3D());               
            }
            if (W.IsReady() && Player.Instance.Spellbook.GetSpell(SpellSlot.W).Name == "LeblancSlideReturn")
            {
                return;
            }


        }


    }

















}
















