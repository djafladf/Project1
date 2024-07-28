import tkinter as tk
from tkinterdnd2 import DND_FILES, TkinterDnD
from PIL import Image
import os


img_origin = any
path = any
def handle_drop(event, label):
    global img_origin
    global path

    path = event.data
    file_path = event.data.strip('{}') 
    file_name = os.path.basename(file_path)

    cnt = file_name.split('.')[0]
    path = path[:len(path)-len(file_name)] + cnt


    try:
        img_origin = Image.open(file_path)
        label.config(text="로드된 파일: " + os.path.basename(file_path)) 
    except Exception as e:
        return


def SaveBT():
    global img_origin
    global path

    curSize = img_origin.size
    pixels = img_origin.load()

    factor = int(entry.get())

    if var.get() == 0:
        new_size = (curSize[0]//factor,curSize[1]//factor)
        new_img = Image.new("RGBA",new_size)
        new_pixels = new_img.load()

        for y in range(new_size[1]):
            for x in range(new_size[0]):
                new_pixels[x,y] = pixels[x * factor, y * factor]
        new_img.save(f"{path}_{factor}_Down.png")
    else:
        new_size = (curSize[0]*factor,curSize[1]*factor)
        new_img = Image.new("RGBA",new_size)
        new_pixels = new_img.load()
        for y in range(new_size[1]):
            for x in range(new_size[0]):
                new_pixels[x,y] = pixels[(int)(x/factor),(int)(y/factor)]
        new_img.save(f"{path}_{factor}_Up.png")
    
    root.destroy()


root = TkinterDnD.Tk()
root.title("이미지 드래그 앤 드롭")
root.geometry("500x500")

frame1 = tk.Frame(root, width=250, height=500, bg="lightgray")
frame1.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

label1 = tk.Label(frame1, text="원본 이미지", bg="lightgray", font=("Arial", 20))
label1.place(relx=0.5, rely=0.5, anchor=tk.CENTER)

entry_label = tk.Label(root, text="Enter a number:")
entry_label.pack()
entry = tk.Entry(root)
entry.pack()

save_button = tk.Button(root, text="저장", command=SaveBT)
save_button.pack()

var = tk.IntVar()
var.set(0)

checkbox = tk.Checkbutton(root,text="사이즈 키우기",variable=var,onvalue=1)
checkbox.pack()

frame1.drop_target_register(DND_FILES)
frame1.dnd_bind('<<Drop>>', lambda event: handle_drop(event, label1))

root.mainloop()
