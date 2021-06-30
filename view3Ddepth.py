#based on https://gist.github.com/Shreeyak/9a4948891541cb32b501d058db227fff

import numpy as np
import os
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import cv2

from PIL import Image
import imageio
#import OpenEXR
import struct

img_nb = 0

depth_path = "./depth_results/"
img_path = "./test/"
filename = "pointcloud"

for file in os.listdir(depth_path):
    if file.endswith(".npy"):

        depth = np.load(os.path.join("./depth_results/", file))

        height = depth.shape[0]
        width = depth.shape[1]

        img = cv2.imread("./test/" + str(img_nb) + ".png")
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        img = cv2.resize(img, (width, height))
        img_array = np.array(img)
        print(img_array.shape)
        #img_flat = img_array.flatten()

        #fig = plt.figure(i)
        #ax = fig.add_subplot(111, projection='3d')

        x = np.arange(height)
        x_mat = np.tile(x, (width, 1))
        xs = x_mat.flatten('F')
        xs = xs.reshape(-1, 1)
        
        y = np.arange(width)
        ys = np.tile(y, height)
        ys = ys.reshape(-1, 1)

        zs = depth.flatten()
        zs = zs.reshape(-1, 1)

        #ax.scatter(xs, ys, zs, c = img_flat)
        #plt.xlabel('xlabel')
        #plt.ylabel('ylabel')

        img_nb = img_nb + 1

        #camera_points = np.array([xs, zs, zs]).transpose(1,2,0).reshape(-1,3)
        #camera_points = np.array([xs, ys, zs]).reshape(-1,3)
        camera_points = np.concatenate((xs, ys, zs), axis=1)

        color_points = img_array.reshape(-1,3)

        assert camera_points.shape == color_points.shape,'Input RGB colors should be Nx3 float array and have same size as input XYZ points'

        # Write header of .ply file
        fid = open(filename + str(img_nb) + ".ply",'wb')
        fid.write(bytes('ply\n', 'utf-8'))
        fid.write(bytes('format binary_little_endian 1.0\n', 'utf-8'))
        fid.write(bytes('element vertex %d\n'%camera_points.shape[0], 'utf-8'))
        fid.write(bytes('property float x\n', 'utf-8'))
        fid.write(bytes('property float y\n', 'utf-8'))
        fid.write(bytes('property float z\n', 'utf-8'))
        fid.write(bytes('property uchar red\n', 'utf-8'))
        fid.write(bytes('property uchar green\n', 'utf-8'))
        fid.write(bytes('property uchar blue\n', 'utf-8'))
        fid.write(bytes('end_header\n', 'utf-8'))

        # Write 3D points to .ply file
        for i in range(camera_points.shape[0]):
            fid.write(bytearray(struct.pack("fffccc",camera_points[i,0],camera_points[i,1],camera_points[i,2],
                                            color_points[i,0].tostring(),color_points[i,1].tostring(),
                                            color_points[i,2].tostring())))
        fid.close()