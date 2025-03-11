#include "LevelComponents.h"
#include "draw_components.h" // For other drawing components
#include "VulkanBuffers.h" // For buffer components

// CPU Level Component construction logic
void Construct_CPULevel(entt::registry& registry, entt::entity entity) {
  auto& cpuLevel = registry.get<CPULevel>(entity);
  
  // Create a log for debugging
  GW::SYSTEM::GLog log;
  log.Create();
  log.EnableConsoleLogging();
  
  // Load the level data from the JSON file
  bool success = cpuLevel.levelData.LoadLevel(
    cpuLevel.jsonPath.c_str(), 
    cpuLevel.modelFolder.c_str(),
    log
  );
  
  if (!success) {
    log.LogCategorized("ERROR", "Failed to load level data");
  } else {
    log.LogCategorized("INFO", "Successfully loaded level data");
  }
}

// GPU Level Component construction logic
void Construct_GPULevel(entt::registry& registry, entt::entity entity) {
  // Try to get the CPULevel component from the same entity
  if (auto* cpuLevel = registry.try_get<CPULevel>(entity)) {
    // 1. Create vertex buffer
    registry.emplace<VulkanVertexBuffer>(entity);
    registry.emplace<std::vector<VERTEX>>(entity, cpuLevel->levelData.verts);
    registry.patch<VulkanVertexBuffer>(entity);
    
    // 2. Create index buffer
    registry.emplace<VulkanIndexBuffer>(entity);
    registry.emplace<std::vector<unsigned int>>(entity, cpuLevel->levelData.indices);
    registry.patch<VulkanIndexBuffer>(entity);
    
    // 3. Create entities for each mesh in the level data
    for (const auto& object : cpuLevel->levelData.blenderObjects) {
      const auto& model = cpuLevel->levelData.models[object.modelIndex];
      
      for (size_t i = 0; i < model.meshes.size(); ++i) {
        const auto& mesh = model.meshes[i];
        
        // Create a new entity for this mesh
        auto meshEntity = registry.create();
        
        // Add GeometryData component
        GeometryData geometryData;
        geometryData.indexStart = model.indexStart + mesh.drawInfo.indexOffset;
        geometryData.indexCount = mesh.drawInfo.indexCount;
        geometryData.materialIndex = model.materialStart + mesh.drawInfo.materialIndex;
        
        registry.emplace<GeometryData>(meshEntity, geometryData);
        
        // Add GPUInstance component
        GPUInstance instance;
        instance.modelMatrix = object.transform;
        instance.colorIndex = geometryData.materialIndex;
        
        registry.emplace<GPUInstance>(meshEntity, instance);
      }
    }
  }
}

// Connect component logic to the registry
CONNECT_COMPONENT_LOGIC() {
  // Connect CPU Level logic
  registry.on_construct<CPULevel>().connect<&Construct_CPULevel>();
  
  // Connect GPU Level logic
  registry.on_construct<GPULevel>().connect<&Construct_GPULevel>();
}
