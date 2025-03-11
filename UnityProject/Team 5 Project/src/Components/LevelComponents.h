#pragma once
#include <string>
#include "load_data_oriented.h" // For Level_Data structure

// CPU Level Component - Stores level data on the CPU
struct CPULevel {
  std::string jsonPath;
  std::string modelFolder;
  Level_Data levelData;
};

// GPU Level Component - Used to create other components
struct GPULevel {
  // This component doesn't need to store anything, 
  // it will only be used to create other components.
};
