import sys
import os
import math
import random
from PIL import Image 
from PIL import ImageDraw
from PIL import ImageFilter

def create_fragment(imgw):
    c = imgw/2
    image = Image.new("RGBA", (imgw, imgw))
    pixels = image.load()
    for ky in range(0, imgw):
        for kx in range(0, imgw):
            dx = kx-c
            dy = ky-c
            dist2 = float(dx*dx + dy*dy)
            dist = math.sqrt(dist2)
            v = float(dist/c)
            v = math.sqrt(v)
            alpha = float(1)-v
            if dist == 0:
                alpha = 1
            #end
            v = int(255*(alpha))
            pixels[kx, ky] = (255, 255, 255, v)
            #pixels[kx, ky] = (v, v, v, v)
        #end
    #end
    return image
#end

def create_fragment_dark(imgw):
    c = imgw/2
    image = Image.new("RGBA", (imgw, imgw))
    pixels = image.load()
    for ky in range(0, imgw):
        for kx in range(0, imgw):
            dx = kx-c
            dy = ky-c
            dist2 = float(dx*dx + dy*dy)
            dist = math.sqrt(dist2)
            v = float(dist/c)
            v *= v
            alpha = float(1)-v
            v = int(255*(alpha))
            v = int(float(v)*2/8)
            pixels[kx, ky] = (255, 255, 255, v)
            #pixels[kx, ky] = (v, v, v, v)
        #end
    #end
    return image
#end

def create(frag_img, frag_dark_img):
    (fw, fh) = frag_img.size
    fpixels = frag_img.load()
    fpixels_dark = frag_dark_img.load()
    imgw = 256
    cx = (imgw-fw)/2
    cy = (imgw-fh)/2
    image = Image.new("RGBA", (imgw, imgw), (255, 255, 255, 0))
    #image = Image.new("RGBA", (imgw, imgw), (0, 0, 0, 255))
    pixels = image.load()
    for j in range(0, 2):
        if j == 0:
            l_near = fw*3/4
            l_far = imgw/4
            num = 100
        else:
            l_near = imgw/4
            l_far = imgw/2+fw/8
            num = 200
        #end
        for i in range(0, num):
            theta = random.uniform(0, math.pi*2)
            l = random.uniform(l_near, l_far)
            if 0 < theta < 0.75:
                l = int(float(l)*0.5)
            elif 1.5 < theta < 2.5:
                l = int(float(l)*0.85)
            elif 3.5 < theta < 4:
                l = int(float(l)*0.75)
            elif 4.5 < theta < 5:
                l = int(float(l)*0.25)
            #end
            
            x = cx + math.cos(theta)*(l-fw/2)
            y = cy + math.sin(theta)*(l-fh/2)
            for ky in range(0, fh):
                for kx in range(0, fw):
                    if x+kx >= imgw or y+ky >= imgw:
                        continue
                    #end
                    (r, g, b, a) = pixels[x+kx, y+ky]
                    if i%4==0:
                        (fr, fg, fb, fa) = fpixels[kx, ky]
                    else:
                        (fr, fg, fb, fa) = fpixels_dark[kx, ky]
                    #end
                    ratio = float(fa)/255
                    #v = int(r*(1-ratio)+fr*ratio)
                    #pixels[x+kx, y+ky] = (v, v, v, 255)
                    v = int(a*(1-ratio)+fa*ratio)
                    pixels[x+kx, y+ky] = (255, 255, 255, v)
                #end
            #end
        #end
    #end
    return image
#end


if __name__ == '__main__':
    random.seed(1234)
    imgw = 32
    frag_img = create_fragment(imgw)
    frag_dark_img = create_fragment_dark(imgw)
    img = create(frag_img, frag_dark_img)
    outdir = "."
    outpath = outdir + "/splash.png"
    img.save(outpath)
    print outpath
#EOF
