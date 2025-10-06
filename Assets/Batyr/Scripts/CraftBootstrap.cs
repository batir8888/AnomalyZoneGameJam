using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Batyr.Scripts
{
    public class CraftBootstrap : Singleton<CraftBootstrap>
    {
        private CraftRepository _repository;

        private void Start()
        {
            if (SaveLoadSystem.Instance.HasSave("seed")) return;
            var seed = (int)DateTime.Now.Ticks;
            SaveLoadSystem.Instance.Save(seed, "seed");

            var tierOne = ArtifactDataFactory.BuildTier1();
            SaveLoadSystem.Instance.Save(tierOne, "tierOne");
            var tierTwo = ArtifactDataFactory.BuildTierK(2, 5, 15, seed, 5);
            SaveLoadSystem.Instance.Save(tierTwo, "tierTwo");
            var tierThree = ArtifactDataFactory.BuildTierK(3, 15, 8, seed, 5);
            SaveLoadSystem.Instance.Save(tierThree, "tierThree");
            var tierFour = ArtifactDataFactory.BuildTierK(4, 8, 5, seed, 5);
            SaveLoadSystem.Instance.Save(tierFour, "tierFour");
            var tierFive = ArtifactDataFactory.BuildTier5(5, seed);
            SaveLoadSystem.Instance.Save(tierFive, "tierFive");

            var mapTierTwo = CraftTierBuilder.BuildTierMap(5, 15, seed);
            SaveLoadSystem.Instance.Save(mapTierTwo, "mapTierTwo");
            var mapTierThree = CraftTierBuilder.BuildTierMap(15, 8, seed);
            SaveLoadSystem.Instance.Save(mapTierThree, "mapTierThree");
            var mapTierFour = CraftTierBuilder.BuildTierMap(8, 5, seed);
            SaveLoadSystem.Instance.Save(mapTierFour, "mapTierFour");
            var mapTierFive = CraftTierBuilder.BuildTierMap(5, 3, seed);
            SaveLoadSystem.Instance.Save(mapTierFive, "mapTierFive");
            
            _repository = new CraftRepository();

            if (TryGetComponent<CraftBootstrapDebugger>(out var debugger))
            {
                debugger.DebugPlease();
            }
        }

        public void Regenerate()
        {
            var seed = (int)DateTime.Now.Ticks;
            SaveLoadSystem.Instance.Save(seed, "seed");

            var tierOne = ArtifactDataFactory.BuildTier1();
            SaveLoadSystem.Instance.Save(tierOne, "tierOne");
            var tierTwo = ArtifactDataFactory.BuildTierK(2, 5, 15, seed, 5);
            SaveLoadSystem.Instance.Save(tierTwo, "tierTwo");
            var tierThree = ArtifactDataFactory.BuildTierK(3, 15, 8, seed, 5);
            SaveLoadSystem.Instance.Save(tierThree, "tierThree");
            var tierFour = ArtifactDataFactory.BuildTierK(4, 8, 5, seed, 5);
            SaveLoadSystem.Instance.Save(tierFour, "tierFour");
            var tierFive = ArtifactDataFactory.BuildTier5(5, seed);
            SaveLoadSystem.Instance.Save(tierFive, "tierFive");

            var mapTierTwo = CraftTierBuilder.BuildTierMap(5, 15, seed);
            SaveLoadSystem.Instance.Save(mapTierTwo, "mapTierTwo");
            var mapTierThree = CraftTierBuilder.BuildTierMap(15, 8, seed);
            SaveLoadSystem.Instance.Save(mapTierThree, "mapTierThree");
            var mapTierFour = CraftTierBuilder.BuildTierMap(8, 5, seed);
            SaveLoadSystem.Instance.Save(mapTierFour, "mapTierFour");
            var mapTierFive = CraftTierBuilder.BuildTierMap(5, 3, seed);
            SaveLoadSystem.Instance.Save(mapTierFive, "mapTierFive");
            
            _repository = new CraftRepository();
        }

        public int GetSeed()
        {
            return _repository.Seed;
        }
        
        public List<ArtifactData> GetArtifacts(int tier)
        {
            return _repository.Artifacts[tier - 1];
        }

        public TierMapping GetTiers(int tier)
        {
            return _repository.Crafts[tier - 1];
        }
    }

    public class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(value.r);
            writer.WritePropertyName("g");
            writer.WriteValue(value.g);
            writer.WritePropertyName("b");
            writer.WriteValue(value.b);
            writer.WritePropertyName("a");
            writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Color(
                obj["r"]?.Value<float>() ?? 0f,
                obj["g"]?.Value<float>() ?? 0f,
                obj["b"]?.Value<float>() ?? 0f,
                obj["a"]?.Value<float>() ?? 1f
            );
        }
    }
}