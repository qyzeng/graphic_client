import socket
import sys
import random
import time
import math

def ReadGts(filename):
	open_file = open(filename,'r')
	read_data = open_file.read(0)
	Line1=open_file.readline()
	split1 = Line1.split()
	N_vertices = split1[0]
	N_edges = split1[1]
	N_triangles = split1[2]
	vertices = [0]*int(N_vertices)*3
	edges = [0]*int(N_edges)*2 # no sure if useful in Unity
	triangles = [0]*int(N_triangles)*3
	triangles_final = [0]*int(N_triangles)*3
	
	for i in range(0,int(N_vertices)):
		line_data = open_file.readline()
		split_data = line_data.split()
		vertices[i*3] = split_data[0]
		vertices[i*3+1] = split_data[1]
		vertices[i*3+2] = split_data[2]

	for i in range(0,int(N_edges)):
		line_data = open_file.readline()
		split_data = line_data.split()
		edges[i*2] = split_data[0]
		edges[i*2+1] = split_data[1]

	for i in range(0,int(N_triangles)):
		line_data = open_file.readline()
		split_data = line_data.split()
		triangles[i*3] = split_data[0]
		triangles[i*3+1] = split_data[1]
		triangles[i*3+2] = split_data[2]

	def getvertices(a,b,c,d):
		vert = [0]*3
		if int(a)!=int(c) and int(a)!=int(d):
			vert[0]=a
			vert[1]=b
		else:
			vert[0]=b
			vert[1]=a
		if int(c)!=int(a) and int(c)!=int(b):
			vert[2]=c
		else:
			vert[2]=d
		return vert

	for i in range(0,int(N_triangles)):
		a = edges[(int(triangles[i*3])-1)*2]
		b = edges[(int(triangles[i*3])-1)*2+1]
		c = edges[(int(triangles[i*3+1])-1)*2]
		d = edges[(int(triangles[i*3+1])-1)*2+1]
		e = getvertices(a,b,c,d)
		triangles_final[i*3] = int(e[0])-1
		triangles_final[i*3+1] = int(e[1])-1
		triangles_final[i*3+2] = int(e[2])-1
	# return number of vertices, vertices,number of triangle and triangles
	return N_vertices,vertices,N_triangles, triangles_final
print 'Mesh reading'
N_vertices,vertices,N_triangles,triangles=ReadGts('bunny.gts')
print 'Mesh reading is done'
# Create a TCP/IP socket

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
            for i in range(0,int(N_vertices)):
                x = vertices[i*3]
                y = vertices[i*3+1]
                z = vertices[i*3+2]
                message = message+"|%s|%s|%s" %(x,y,z)
                
            for i in range(0,int(N_triangles)):
                x = triangles[i*3]
                y = triangles[i*3+1]
                z = triangles[i*3+2]
                message = message+"|%s|%s|%s" %(x,y,z)
            connection.sendall(message+"\r\n")    
            mesh_no = connection.recv(16)
            print >>sys.stderr, 'Object Number Mesh: "%s"' % (mesh_no)

    finally:
        # Clean up the connection
        connection.close()
