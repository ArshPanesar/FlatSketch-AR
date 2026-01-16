# AR Canvas Painting System

This is an AR project that transforms real-world flat surfaces into interactive virtual canvases. 
Built using Unity and AR Foundation, the system allows users to place, manipulate, and paint on spatially anchored canvases using colors, textures, and GIFs in real time.

This project was developed as a final project for a VR/AR course.

## Features

- **Plane-Based Canvas Placement**
  - Detects real-world flat surfaces using AR plane detection
  - Converts detected planes into rectangular, spatially anchored canvases
  - Supports multiple canvases per environment

- **Interactive AR Painting**
  - Color-based painting with adjustable brush size
  - Texture and image stamping at touch hit points
  - Smooth brush strokes via touch-path interpolation

- **GPU-Accelerated Rendering**
  - Brush Painting implemented using compute shaders
  - Batched paint operations to reduce GPU dispatch overhead
  - Optimized thread group sizes for mobile hardware (tested on an Android Device)

- **Canvas Editing Tools**
  - Reposition, rotate, and resize canvases in AR space
  - Duplicate canvases to compensate for unstable plane detection

## Architecture

![Architecture Diagram](https://github.com/user-attachments/assets/e18be5c4-a03b-42b3-800f-7f749a3d81c0)

## Screenshots

> <img width="1555" height="1435" alt="PaintingCanvases" src="https://github.com/user-attachments/assets/9dfe2550-b314-4dbd-ae52-08115c489ddd" />

<br>

> <img width="1411" height="1431" alt="TexturedCanvases" src="https://github.com/user-attachments/assets/f5ee803b-e1a6-4f81-905a-f2c45c6ff806" />

<br>

> <img width="1583" height="1435" alt="CanvasResizing" src="https://github.com/user-attachments/assets/97c998f6-faf5-4807-8f60-0c06c403dcf8" />

<br>

> <img width="1473" height="1439" alt="GIFsOnCanvases" src="https://github.com/user-attachments/assets/365ed8b9-4f51-43f8-9afd-fa2004742b43" />

