using System;
using System.Collections.Generic;
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


namespace Trynda0
{


    class Program
    {
        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;

        }

        private static AIHeroClient Trynda = ObjectManager.Player;

        private static AIHeroClient User = Player.Instance;

        private static Spell.Active Q;

        private static Spell.Targeted W;

        private static Spell.Skillshot E;

        private static Spell.Active R;

        private static Item bilgewater, botrk, tiamat, hydra, Youmuu;


        private static Item HealthPotion, HuntersPotion, Biscuit, CorruptPotion, RefillPotion;
        
        



        private static Menu TryndaMenu, ComboMenu, LaneClearMenu, JungleClearMenu, FleeMenu, ItemMenu, DrawingsMenu, SkinChangerMenu;





        private static List<Spell.SpellBase> SpellList = new List<Spell.SpellBase>();







        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (User.ChampionName != "Tryndamere")
            {
                return;
            }

            Q = new Spell.Active(spellSlot: SpellSlot.Q);

            W = new Spell.Targeted(spellSlot: SpellSlot.W, spellRange: 850);

            E = new Spell.Skillshot(spellSlot: SpellSlot.E, spellRange: 660, skillShotType: SkillShotType.Circular, spellSpeed: 700, spellWidth: 225);

            R = new Spell.Active(spellSlot: SpellSlot.R);



            






            bilgewater = new Item(3144, 550);
            botrk = new Item(3153, 550);
            tiamat = new Item(3077, 400);
            hydra = new Item(3074, 400);
            Youmuu = new Item(3142, 800);





            HealthPotion = new Item(2003, 0);
            Biscuit = new Item(2010, 0);
            CorruptPotion = new Item(2033, 0);
            RefillPotion = new Item(2031, 0);
            HuntersPotion = new Item(2032, 0);




            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);



             

            Chat.Print("TryndaZero loaded successfully");



            CurrentSkinID = User.SkinId;
            
        
            








            TryndaMenu = MainMenu.AddMenu("TryndaZero", "TryndaZero");

            ComboMenu = TryndaMenu.AddSubMenu("Combo");
            ComboMenu.AddGroupLabel("Combo Settings");           
            ComboMenu.Add("W", new CheckBox("Use W"));
            ComboMenu.Add("E", new CheckBox("Use E"));
            ComboMenu.AddSeparator();
            ComboMenu.Add("UseE", new CheckBox("Use E Only if out of AA Range"));
            ComboMenu.AddSeparator(50);
            ComboMenu.AddGroupLabel("Auto Q Settings");    
            ComboMenu.Add("Q", new CheckBox("Auto Use Q"));
            ComboMenu.Add("UseQ", new Slider("% HP to cast Q", 70, 1, 100));
            ComboMenu.AddSeparator(50);
            ComboMenu.AddGroupLabel("Auto R Settings(Use Key for quick changing)");
            ComboMenu.Add("R", new KeyBind("Auto Use R(Enemy in 1200 Range)",true,KeyBind.BindTypes.PressToggle, 'L'));
            ComboMenu.Add("AR", new CheckBox("Always Auto Use R", false));
            ComboMenu.Add("UseR", new Slider("% HP to cast R", 20, 1, 100));
            ComboMenu.AddSeparator();            


            LaneClearMenu = TryndaMenu.AddSubMenu("LaneClear");
            LaneClearMenu.AddGroupLabel("LaneClear Settings");
            LaneClearMenu.Add("E", new CheckBox("Use E"));
            LaneClearMenu.AddSeparator();
            LaneClearMenu.Add("Elc", new Slider("Minions to use E >=", 3, 1, 10));


            JungleClearMenu = TryndaMenu.AddSubMenu("JungleClear");
            JungleClearMenu.AddGroupLabel("Jungle Clear Settings");
            JungleClearMenu.Add("EJ", new CheckBox("Use E"));

            FleeMenu = TryndaMenu.AddSubMenu("Flee");
            FleeMenu.AddGroupLabel("Flee Settings");
            FleeMenu.Add("EF", new CheckBox("Use E"));

            ItemMenu = TryndaMenu.AddSubMenu("Items");
            ItemMenu.AddGroupLabel("Item Settings");
            ItemMenu.AddGroupLabel("Combo:");
            ItemMenu.Add("bilge", new CheckBox("Use Bilgewater Cutlass in Combo"));
            ItemMenu.Add("youmuu", new CheckBox("Use Youmuu's in Combo"));
            ItemMenu.Add("tia", new CheckBox("Use Tiamat in Combo"));
            ItemMenu.Add("hydra", new CheckBox("Use Hydra in Combo"));
            ItemMenu.Add("botrk", new CheckBox("Use BotrK in Combo"));
            ItemMenu.Add("bork", new Slider("%HP to use BotrK <=", 75, 0, 100));
            ItemMenu.AddSeparator();
            ItemMenu.AddGroupLabel("LaneClear/JungleClear:");
            ItemMenu.Add("tialc", new CheckBox("Use Tiamat in Laneclear/JungleClear"));
            ItemMenu.Add("hydralc", new CheckBox("Use Hydra in Laneclear/JungleClear"));
            ItemMenu.AddSeparator(50);
            ItemMenu.Add("potion", new CheckBox("Auto Use Potion"));
            ItemMenu.Add("potionhp", new Slider("Use Auto Potion when %HP <=", 50, 0, 100));





            DrawingsMenu = TryndaMenu.AddSubMenu("Drawings");
            DrawingsMenu.AddGroupLabel("Drawing Settings");
            DrawingsMenu.Add("DrawW", new CheckBox("Draw W"));
            DrawingsMenu.Add("DrawE", new CheckBox("Draw E"));




            SkinChangerMenu = TryndaMenu.AddSubMenu("Skin Changer");
            SkinChangerMenu.Add("EnableSkin", new CheckBox("Enable Skinchanger", false));
            SkinChangerMenu.Add("SkinID", new Slider("Skin ID", 1, 0, 8));
            










            Drawing.OnDraw += Drawing_OnDraw;



            





            Game.OnTick += Game_OnTick;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
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


            AutoPot();




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
            if(User.IsDead || MenuGUI.IsChatOpen || User.IsRecalling())
            {
                return;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
                Items();

            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee))
            {
                Flee();
            }

            if(Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }



            if (SkinChangerMenu["EnableSkin"].Cast<CheckBox>().CurrentValue)
                {
                if (SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue != Player.Instance.SkinId)
                {
                    Player.SetSkinId(SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue);
                }
            }
            else if(!SkinChangerMenu["EnableSkin"].Cast<CheckBox>().CurrentValue)
            {
                Player.SetSkinId(CurrentSkinID);
                SkinChangerMenu["SkinID"].Cast<Slider>().CurrentValue = CurrentSkinID;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }

            if(ComboMenu["R"].Cast<KeyBind>().CurrentValue)
            {
                ComboMenu["AR"].Cast<CheckBox>().CurrentValue = false;
            }
            if(!ComboMenu["R"].Cast<KeyBind>().CurrentValue)
            {
                ComboMenu["AR"].Cast<CheckBox>().CurrentValue = true;
            }



            if (ComboMenu["AR"].Cast<CheckBox>().CurrentValue && Player.Instance.HealthPercent <= ComboMenu["UseR"].Cast<Slider>().CurrentValue && R.IsReady())
            {
                R.Cast();
            }

            if (!ComboMenu["AR"].Cast<CheckBox>().CurrentValue && ComboMenu["R"].Cast<KeyBind>().CurrentValue && Player.Instance.HealthPercent <= ComboMenu["UseR"].Cast<Slider>().CurrentValue && R.IsReady())
            {
                if (ObjectManager.Get<AIHeroClient>().Any(z => z.IsEnemy && z.Distance(Trynda.Position) <= 1200))
                {
                    R.Cast();


                }
            }


            if (ComboMenu["Q"].Cast<CheckBox>().CurrentValue && Player.Instance.HealthPercent <= ComboMenu["UseQ"].Cast<Slider>().CurrentValue && Q.IsReady())
            {
                
                {
                    Q.Cast();
                }
                

            }


        }




        private static int CurrentSkinID;




        private static void Combo()
        {

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);





            if (target == null)
            {
                return;
            }



            if (ComboMenu["W"].Cast<CheckBox>().CurrentValue)
            {
                var Wcomboenemy = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                if (target.IsValidTarget(W.Range) && W.IsReady() && !Wcomboenemy.IsFacing(Trynda))
                {
                    W.Cast(target);

                }


            }





            if (ComboMenu["E"].Cast<CheckBox>().CurrentValue)
            {
                if (ComboMenu["E"].Cast<CheckBox>().CurrentValue && ComboMenu["UseE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    if (target.IsValidTarget() && Trynda.Distance(target) <= User.GetAutoAttackRange(User))
                    {
                        return;
                    }


                    else
                    {
                        E.Cast(target);
                    }
                }
                else if (ComboMenu["E"].Cast<CheckBox>().CurrentValue && !ComboMenu["UseE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        E.Cast(target);
                    }
                }








            }
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










        private static void Items()
        {
            var itemen2 = TargetSelector.GetTarget(200, DamageType.Physical);
            var itemen = TargetSelector.GetTarget(550, DamageType.Physical);
            if (itemen != null)
            {

                if (ItemMenu["botrk"].Cast<CheckBox>().CurrentValue && Item.HasItem(botrk.Id) && Item.CanUseItem(botrk.Id) && Player.Instance.HealthPercent <= ItemMenu["bork"].Cast<Slider>().CurrentValue)
                {
                    Item.UseItem(botrk.Id, itemen);
                }
                    if (ItemMenu["bilge"].Cast<CheckBox>().CurrentValue && Item.HasItem(bilgewater.Id) && Item.CanUseItem(bilgewater.Id))
                {
                    Item.UseItem(bilgewater.Id, itemen);
                }
                if (ItemMenu["tia"].Cast<CheckBox>().CurrentValue && Item.HasItem(tiamat.Id) && Item.CanUseItem(tiamat.Id))
                {
                    Item.UseItem(tiamat.Id, itemen2);
                }
                if (ItemMenu["hydra"].Cast<CheckBox>().CurrentValue && Item.HasItem(hydra.Id) && Item.CanUseItem(hydra.Id))
                {
                    Item.UseItem(hydra.Id, itemen2);
                }
            }
                if (ItemMenu["youmuu"].Cast<CheckBox>().CurrentValue)
            {
                if (Youmuu.IsOwned() && Youmuu.IsReady() && Youmuu.IsInRange(itemen))
                {
                    Youmuu.Cast();
                }
            }



        }  
            
            
            
            
                    





        
        ////////
        ////////
        ////////
        ////////
        ////////
        ////////
        ////////






        private static void LaneClear()
        {
            var UseE = LaneClearMenu["Elc"].Cast<Slider>().CurrentValue;


            if (LaneClearMenu["E"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
                var pred = EntityManager.MinionsAndMonsters.GetLineFarmLocation(target, 225, (int)E.Range);
                

                if (pred.HitNumber >= UseE)
                {
                    E.Cast(pred.CastPosition);
                }
            }
            

            if (ItemMenu["tialc"].Cast<CheckBox>().CurrentValue)
            {
                var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
                var pred1 = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(target, 400, 400);
                if (pred1.HitNumber >= UseE)
                {
                    tiamat.Cast();
                }

            }

            if (ItemMenu["hydralc"].Cast<CheckBox>().CurrentValue)
            {
                var target = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget() && E.IsInRange(x));
                var pred1 = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(target, 400, 400);
                if(pred1.HitNumber >= UseE)
                {
                    hydra.Cast();
                }



            }

        }












        private static void JungleClear()
        {
            var monsters = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, E.Range).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
            foreach (var m in monsters)
            {
                if (JungleClearMenu["EJ"].Cast<CheckBox>().CurrentValue)
                {
                    E.Cast(m);
                }



            }

            var monster1 = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, 200).Where(t => !t.IsDead && t.IsValid && !t.IsInvulnerable);
                foreach (var m in monster1)
            {
                if(ItemMenu["tialc"].Cast<CheckBox>().CurrentValue)
                {
                    tiamat.Cast(m);
                }

                if(ItemMenu["hydralc"].Cast<CheckBox>().CurrentValue)
                {
                    hydra.Cast(m);
                }


            }






        }










        private static void Flee()
        {
            if (FleeMenu["EF"].Cast<CheckBox>().CurrentValue)
            {
                E.Cast(Player.Instance.ServerPosition.Extend(Game.CursorPos, E.Range).To3D());
            }
           }

    }

















        }
    





        






    
    

