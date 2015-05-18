#!/usr/bin/env python
import math
import numpy
import scipy.special
import scipy.misc
import pyqtgraph as pg

x,y,z = numpy.ogrid[-50:50:51j,-50:50:51j,-50:50:51j]
def GetWaveFunction(n,l,m,x,y,z):
    r = lambda x,y,z: numpy.sqrt(x**2+y**2+z**2)
    theta = lambda x,y,z: numpy.arccos(z/r(x,y,z))
    phi = lambda x,y,z: numpy.arctan(y/x)
    a0 = 1.
    R = lambda r,n,l: (2*r/n/a0)**l * numpy.exp(-r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*r/n/a0)
    WF = lambda r,theta,phi,n,l,m: R(r,n,l) * scipy.special.sph_harm(m,l,phi,theta)
    absWF = lambda r,theta,phi,n,l,m: abs(WF(r,theta,phi,n,l,m))**2
    w = absWF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    w[numpy.isnan(w)]=0
    return w
isovalue = 0.45
absw = GetWaveFunction(4,3,0,x,y,z)
verts, faces = pg.isosurface(absw,isovalue)
N_vertices = int(verts.size/3)
N_triangles = int(faces.size/3)

import socket
import sys
import random
import time
import math
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('localhost', 51111)
print >>sys.stderr, 'starting up on %s port %s' % server_address
sock.bind(server_address)
# Listen for incoming connections
sock.listen(1)
while True:
    # Wait for a connection
    print >>sys.stderr, 'waiting for a connection'
    connection, client_address = sock.accept()
    try:
        print >>sys.stderr, 'connection from', client_address

        # Receive the data in small chunks and retransmit it
        while True:
            data = connection.recv(16)
            print >>sys.stderr, 'received "%s"' % data
            if data:
                print >>sys.stderr, 'sending data back to the client'
            else:
                print >>sys.stderr, 'no more data from', client_address
                break
            #connection.sendall("L,simple_coordinates\r\n")
            #data = connection.recv(16)
            #connection.sendall('C,0,0,0,1\r\n')
            #generate a egg carton mesh
            message= 'R|%s|%s' %(N_vertices,N_triangles)
            for i in range(0,N_vertices):
                x = 0 if verts[i][0] is 'nan' else verts[i][0]
                y = 0 if verts[i][1] is 'nan' else verts[i][1]
                z = 0 if verts[i][2] is 'nan' else verts[i][2]
                message = message+"|%s|%s|%s" %(x,y,z)

            for i in range(0,N_triangles):
                v1 = faces[i][0]
                v2 = faces[i][1]
                v3 = faces[i][2]
                message = message+"|%s|%s|%s" %(v1,v2,v3)
            connection.sendall(message+"\r\n")
            mesh_no = connection.recv(16)
            print >>sys.stderr, 'Object Number Mesh: "%s"' % (mesh_no)
            message = 'd|%s|%s'%(int(mesh_no),isovalue)
            connection.sendall(message+"\r\n")
            data = connection.recv(16)
            #animate the mesh data
    finally:
        # Clean up the connection
        connection.close()
