Engine Dust Kicker for Kerbal Space Program


A demonstration project for spawning and controlling dust effects created on the ground where engine exhaust hits. It currently is a part module which should be on the same part as an engine. Originally concived to handle each nozzle from an engine seperately (which is why there are so many lists of engines), multiple nozzle support is just a very basic framework, only tested on conventional one nozzle engines.

Variables:

effectName			Used for searching Multi_Model_Effects, which are what mod makers are developing. While this works, it requires the mod maker to have assigned a custom name for each effect. In CreateEffect3(), the plugin will print all of the Multi_Model_Effect names to the console/log.


effectURL			An attempt to find the effect by the game's GameDatabase. I don't think this is working right now...

useURL				bool flag. True means the plugin will search for the effect by URL, false means it will search by effectName (the internal name of the effect)

groundInterfaceTransform	Transform created where the ground (terrain and permanent objects, e.g. the launchpad) is hit by a ray extending from the thrustTransform of the engine. The center of the effect.



Method Comments:

OnActive		gotoLeave was used to both restrict the plugin to finding one engine and also because it would give mismatched type errors

OnUpdate		hardcoded using the first item in the engineModuleList, would need to be changed to support multi engines and multi-nozzle engines

DustController		finds the location of groundInterfaceTransform and applies the effects, scaling relative to the inverse square of the distance to the ground and linearly with the thrust of the engine. I don't know if this is working correctly.

			layerMask = 1 << 15 is the terrain/permanent objects
			the "100" in the Physics.Raycast call is the distance of the ray in meters (i think) to give a max distance. Haven't tested closely enough to see if this works as intended 

CreateEffect2		used for the URL effects. Not 100% functional

CreateEffect3		dustEffect.fxEmitters.Add(Emitter(item.pe)) Extracts the particle emitter (the .pe) from the KSPParticleEmitter container (item) and uses that in the conventional way.



Other:

I've been unable to simply use the Part.Effect() method unless there is a spare transform attached to the model which I can move via code (I've been unable to move the thrustTransform to the ground, apply the dust effect, and then move the transform back for applying thrust). I've been unsuccessful adding a transform to the model at runtime because I can't find the model in the code, but if able it may be the simplest way (you retain the KSPParticleEffect container then) to apply new effects.