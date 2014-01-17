using System;
using System.Linq;
using System.Collections.Generic;
using KSP;
using UnityEngine;

namespace EngineDustKicker
{
    public class EngineDustKicker : PartModule
    {
        [KSPField]
        public string effectName = "effectName";

        [KSPField]
        public string effectURL = "";

        [KSPField]
        public bool useURL = false;

        public List<ModuleEngines> engineModuleList;

        public Transform groundInterfaceTransform;

        //public GameObject go;

        public PrefabParticleFX groundFX;

        public FXGroup dustEffect;

        public List<KSPParticleEmitter> dustPE;

        public override void OnAwake()
        {
            //groundInterfaceTransform = new GameObject().transform;

            //go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //go.collider.enabled = false;

            groundInterfaceTransform = new GameObject("groundTransform").transform;

            groundInterfaceTransform.transform.name = "groundTransform";

            print("created transform");


            //CreateEffect2();
        }

        public override void OnActive()
        {
            engineModuleList.Clear();

            print("gotocreateeffect");

            if (useURL)
                CreateEffect2();
            else
                CreateEffect3();


            print("past onActive1");


            foreach (ModuleEngines pm in this.part.Modules)
            {
                //if (moduleList.Contains(pm.moduleName)) //then the part also contains an "approved" engine module
                //{

                if (pm.moduleName == "ModuleEngines")
                {
                    engineModuleList.Add(pm);

                    goto Leave;
                }
                //}
            }

        Leave:
            ;



            //ReInitFX();

        }

        public override void OnUpdate()
        {
            if (this.vessel.terrainAltitude < 100) //only run if within 100m of the ground to reduce cpu usage
            {
                if (engineModuleList.Count > 0)
                {
                    DustController(engineModuleList[0], engineModuleList[0].thrustTransforms);
                }
            }
        }

        public void DustController(ModuleEngines pm, List<Transform> lt)
        {
            float thrustPer = pm.finalThrust / lt.Count;

            for (int i = 0; i < lt.Count; i++)
            {
                var layerMask = 1 << 15;

                Ray r = new Ray(lt[i].position, lt[i].forward);

                RaycastHit ht;
                Physics.Raycast(r, out ht, 100, layerMask);

                float pwr = 0;

                pwr = Mathf.Clamp01((pm.finalThrust / lt.Count) / 200); //if the engine produces 200 kN of thrust, then the effect is a "full power". obviously this can be changed
                pwr = Mathf.Clamp01((ht.distance / 100 - 1f) * (ht.distance / 100 - 1f)) * pwr; //this is intended to make the effect larger as the distance is reduced. this can of course also be changed.

                groundInterfaceTransform.position = ht.point;
                groundInterfaceTransform.forward = ht.normal;


                foreach (ParticleEmitter fx in dustEffect.fxEmitters)
                {
                    fx.transform.position = ht.point;
                    fx.transform.forward = ht.normal;
                }

                dustEffect.Power = pwr;

                if (pwr > .01)
                {
                    dustEffect.setActive(true);

                    foreach (ParticleEmitter fx in dustEffect.fxEmitters)
                    {
                        fx.gameObject.SetActive(true);
                    }
                }

                else
                {
                    dustEffect.setActive(false);

                    foreach (ParticleEmitter fx in dustEffect.fxEmitters)
                    {
                        fx.gameObject.SetActive(false);
                    }
                }


            }
        }

        public void CreateEffect2()
        {
            UnityEngine.Object m = GameDatabase.Instance.GetModel(effectURL);

            print(m.GetType().ToString());

            KSPParticleEmitter pe = new KSPParticleEmitter();

            print(pe.GetType().ToString());

            Type t = m.GetType();



            //if (m.GetType() == typeof(KSPParticleEmitter))
            //{
            //    print(m.name + " is a KSPParticleEmitter");

            //    KSPParticleEmitter mPe = (KSPParticleEmitter)m;

            //    dustEffect.fxEmitters.Add(Emitter(mPe.pe));
            


            //;
        }

        public void CreateEffect3()
        {
            ///under construction, super derpy way of trying to get it to find the correct effects

            dustEffect.fxEmitters.Clear();

            var pel = Resources.FindObjectsOfTypeAll(typeof(KSPParticleEmitter));

            for (int i = 0; i < pel.Count(); i++)
            {

                KSPParticleEmitter item = (KSPParticleEmitter)pel.GetValue(i);

                print("KSPParticleEmitter found! " + item.name);

                if (item.name == effectName)
                {
                    print("found a particle emitter with name " + item.name);

                    //dustPE.Add(item);
                    dustEffect.fxEmitters.Add(Emitter(item.pe));
                }
            }
        }

        public ParticleEmitter Emitter(ParticleEmitter pe)
        {
            ParticleEmitter fx;

            fx = (ParticleEmitter)UnityEngine.Object.Instantiate(pe);

            fx.transform.parent = groundInterfaceTransform;
            fx.transform.position = groundInterfaceTransform.position;
            fx.transform.rotation = groundInterfaceTransform.rotation;
            fx.gameObject.SetActive(false);

            return fx;

        }
    }
}
