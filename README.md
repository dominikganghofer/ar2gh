# AR2GH

A Grasshopper plug-in that receives and displays spatial information from an augmented reality device.



## Features

- **LiDAR Point Cloud**

  - Generates and streams a dense, colored point cloud of the environment

  - Requires a newer Android phone or an iOS device equipped with a LiDAR scanner

  - For the the cloud generation in Unity [this](https://github.com/TakashiYoshinaga/iPad-LiDAR-Depth-Sample-for-Unity) project by [TakashiYoshinaga](https://github.com/TakashiYoshinaga) was very helpful

  - In Grasshopper the point cloud is stored using [Tarsier](https://bitbucket.org/camnewnham/tarsier/src/master/)'s `GH_PointCloud` and can be modified and rendered using Tarsier

     

<img src="ar2ghScan2.gif" style="zoom:100%;" />



- **Plane Detection**

  - Streams the plane surfaces detected in the environment

  - Planes are classified as *Wall, Floor, Ceiling, Table, Seat, Door, Window or None*

    

- **Meshing**
  - Streams a triangle mesh that corresponds to the physical space
  - The mesh has vertex colors that are generated from the camera image
  - To reduce the amount of streamed data this feature cannot be used simultaneously with the **LiDAR streaming** 
  - 

- **In Progress:**
  - [ ] Device Tracking
  - [ ] Light estimation
  - [ ] Environment probes
  - [ ] Face tracking
  - [ ] 3D Body tracking
  - [ ] 2D Image tracking
  - [ ] 3D Object tracking



## Getting Started

- **Unity Project:**
  - build `\UnityProject\ARDataStreamer` on a device compatible with Unity's AR-Foundation
  - run app and enter IP address of the device that runs grasshopper

- **Grasshopper Project:**
  - if you have not created a custom Grasshopper component before, start from [here](https://developer.rhino3d.com/guides/grasshopper/)
  - build and install `\GrasshopperProject\AR2GH`
  - install [Tarsier](https://www.food4rhino.com/app/tarsier) for better point cloud rendering
  - in Grasshopper create a `device` component to connect to the augmented reality device

- Make sure both devices are in the same network ⚠️

  

## License

```
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
```



## References

- LiDAR Point Cloud Generation [GitHub - TakashiYoshinaga/iPad-LiDAR-Depth-Sample-for-Unity](https://github.com/TakashiYoshinaga/iPad-LiDAR-Depth-Sample-for-Unity)

- Tarsier https://bitbucket.org/camnewnham/tarsier/src/master/ 