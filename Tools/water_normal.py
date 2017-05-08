import sys
import random
import math
from PIL import Image, ImageDraw

def make_table():
    ptbl = []
    for i in range(256):
        ptbl.append(random.randrange(0, 256))
    #end
    for i in range(256):
        ptbl.append(ptbl[i])
    #end
    return ptbl
#end

def fade(t):
    return t*t*t * (t * (t*6.0 - 15.0) + 10.0)
#end

def inc(num, repeat):
    num = num + 1
    if repeat > 1:
        num %= repeat
    #end
    return num
#end


def grad_another_implementatoin(hash, x, y, z):
    h = hash & 15
    if h < 8:
        u = x
    else:
        u = y
    #end
    if h < 4:
        v = y
    elif (h == 12 or h == 14):
        v = x
    else:
        v = z
    #end
    if h&1 == 0:
        u0 = u
    else:
        u0 = -u
    #end
    if h&2 == 0:
        v0 = v
    else:
        v0 = -v
    #end
    return u0 + v0
#end

def grad(hash, x, y, z):
    h = hash & 0xf
    if h == 0x0:
        return  x + y
    elif h == 0x1:
        return -x + y
    elif h == 0x2:
        return  x - y
    elif h == 0x3:
        return -x - y
    elif h == 0x4:
        return  x + z
    elif h == 0x5:
        return -x + z
    elif h == 0x6:
        return  x - z
    elif h == 0x7:
        return -x - z
    elif h == 0x8:
        return  y + z
    elif h == 0x9:
        return -y + z
    elif h == 0xa:
        return  y - z
    elif h == 0xb:
        return -y - z
    elif h == 0xc:
        return  y + x
    elif h == 0xd:
        return -y + z
    elif h == 0xe:
        return  y - x
    elif h == 0xf:
        return -y - z
    else:
        return 0
    #end
#end


def lerp(a, b, x):
    return a + x * (b - a)
#end

def perlin(x, y, z, ptbl, repeat):
    if repeat > 1:
        x = x % repeat
        y = y % repeat
        z = z % repeat
    #end

    xi = int(x) % 256
    yi = int(y) % 256
    zi = int(z) % 256
    xf = x - int(x)
    yf = y - int(y)
    zf = z - int(z)

    u = fade(xf)
    v = fade(yf)
    w = fade(zf)    

    r = repeat
    p = ptbl
    aaa = p[p[p[      xi ]+      yi ]+      zi ]
    aba = p[p[p[      xi ]+inc(yi,r)]+      zi ]
    aab = p[p[p[      xi ]+      yi ]+inc(zi,r)]
    abb = p[p[p[      xi ]+inc(yi,r)]+inc(zi,r)]
    baa = p[p[p[inc(xi,r)]+      yi ]+      zi ]
    bba = p[p[p[inc(xi,r)]+inc(yi,r)]+      zi ]
    bab = p[p[p[inc(xi,r)]+      yi ]+inc(zi,r)]
    bbb = p[p[p[inc(xi,r)]+inc(yi,r)]+inc(zi,r)]
 
    x1 = lerp(grad(aaa, xf  , yf  , zf),
              grad(baa, xf-1, yf  , zf),
              u)
    x2 = lerp(grad(aba, xf  , yf-1, zf),
              grad(bba, xf-1, yf-1, zf),
              u)
    y1 = lerp(x1, x2, v)
 
    x1 = lerp(grad(aab, xf  , yf  , zf-1),
              grad(bab, xf-1, yf  , zf-1),
              u)
    x2 = lerp(grad(abb, xf  , yf-1, zf-1),
              grad(bbb, xf-1, yf-1, zf-1),
              u)
    y2 = lerp (x1, x2, v)
 
    return (lerp (y1, y2, w)+1)/2

#end

def make_image(pixels, imgx, imgy, level, r, ptbl):
    for ky in range(imgy):
        for kx in range(imgx):
            v = perlin(float(kx)/level, float(ky)/level, 0, ptbl, imgx/level)
            c = int(v*r*255)
            c0 = pixels[kx, ky][0]
            pixels[kx, ky] = (c+c0, c+c0, c+c0)
        #end
    #end
#end

def rep(v, range):
    if v < 0:
        return int(v + range)
    elif v >= range:
        return int(v - range)
    else:
        return v
    #end
#end

def make_normalmap(pixels2, pixels, imgx, imgy):
    for ky in range(imgy):
        for kx in range(imgx):
            vl = float(pixels[rep(kx,imgx), ky][0])/255
            vr = float(pixels[rep(kx+1,imgx), ky][0])/255
            vx = -(vl - vr)*128
            vt = float(pixels[kx, rep(ky,imgy)][0])/255
            vb = float(pixels[kx, rep(ky+1,imgy)][0])/255
            vz = -(vt - vb)*128
            vy = 1
            len2 = vx*vx + vy*vy + vz*vz
            rlen = float(1)/math.sqrt(len2)
            vxi = int((vx * rlen * 128)+127)
            vyi = int((vy * rlen * 128)+127)
            vzi = int((vz * rlen * 128)+127)
            pixels2[kx, ky] = (vxi, vyi, vzi)
        #end
    #end
#end

def make_worley_image(imgx, imgy, layer):
    imgz = imgx
    image = Image.new("RGB", (imgx, imgy))
    pixels = image.load()
    draw = ImageDraw.Draw(image)
#    n = 3200 # of seed points
    n = 800 # of seed points
    random.seed(123456)
    seedsX = [random.randint(0, imgx - 1) for i in range(n)]
    seedsY = [random.randint(0, imgy - 1) for i in range(n)]
    seedsZ = [random.randint(0, imgx - 1) for i in range(n)]
    distbuf = [[0 for i in range(imgx)] for j in range(imgy)]
    zvalue = layer * imgz

    maxDist = 0.0
    for ky in range(imgy):
        for kx in range(imgx):
            mindist = sys.maxsize
            for i in range(n):
                dx = seedsX[i] - kx
                if dx > imgx/2:
                    dx -= imgx
                elif dx < -imgx/2:
                    dx += imgx
                #end
                dy = seedsY[i] - ky
                if dy > imgy/2:
                    dy -= imgy
                elif dy < -imgy/2:
                    dy += imgy
                #end
                dz = seedsZ[i] - zvalue
                if dz > imgz/2:
                    dz -= imgz
                elif dz < -imgz/2:
                    dz += imgz
                #end
                len = math.sqrt(dx*dx + dy*dy + dz*dz)
                if len < mindist:
                    mindist = len
                #end
            #end
            distbuf[kx][ky] = mindist
            if mindist > maxDist:
                maxDist = mindist
            #end
        #end
    #end
    
    for ky in range(imgy):
        for kx in range(imgx):
            dist = distbuf[kx][ky]
            c = int(round(255 * dist / maxDist))
            pixels[kx, ky] = (c, c, c) 
        #end
    #end
    return image
#end

def add_pixels(pixels, image_worley, imgx, imgy):
    pixels0 = image_worley.load()
    fa = 0.5
    fb = 1.0-fa
    for ky in range(imgy):
        for kx in range(imgx):
            v0 = int(pixels[kx, ky][0]*fa + pixels0[kx, ky][0]*fb)
            if v0 > 255: v0 = 255
            v1 = int(pixels[kx, ky][1]*fa + pixels0[kx, ky][1]*fb)
            if v1 > 255: v1 = 255
            v2 = int(pixels[kx, ky][2]*fa + pixels0[kx, ky][2]*fb)
            if v2 > 255: v2 = 255
            pixels[kx, ky] = (v0, v1, v2)
        #end
    #end
#end

def main():
    random.seed(123456)
    ptbl = make_table()
    imgx = 256; imgy = 256 # image size
#    imgx = 64; imgy = 64 # image size
    image = Image.new("RGB", (imgx, imgy))
    draw = ImageDraw.Draw(image)
    pixels = image.load()
    r = float(1)/4
    #make_image(pixels, imgx, imgy, imgx/4, r, ptbl)
    # make_image(pixels, imgx, imgy, imgx/8, r, ptbl)
    make_image(pixels, imgx, imgy, imgx/10, r, ptbl)
    make_image(pixels, imgx, imgy, imgx/16, r*2, ptbl)
    make_image(pixels, imgx, imgy, imgx/32, r, ptbl)
    # make_image(pixels, imgx, imgy, imgx/64, r*2, ptbl)
    image.save("PerlinNoise.png", "PNG")

    image_worley = make_worley_image(imgx, imgy, 0)
    image_worley.save("WorleyNoise.png", "PNG")
    add_pixels(pixels, image_worley, imgx, imgy)

    image2 = Image.new("RGB", (imgx, imgy))
    draw2 = ImageDraw.Draw(image2)
    pixels2 = image2.load()
    make_normalmap(pixels2, pixels, imgx, imgy)
    image2.save("water_normal.png", "PNG")
#end

main()
