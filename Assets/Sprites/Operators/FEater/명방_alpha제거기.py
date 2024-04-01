import tkinter as tk
from tkinterdnd2 import DND_FILES, TkinterDnD
from PIL import Image
import os


def SaveAlpha():
    data = img_alpha.getdata()
    new_data = []
    for item in data:
        # 검은색 픽셀은 알파 값을 0으로 설정하여 완전히 투명하게 만듭니다.
        if item[0] == 0 and item[1] == 0 and item[2] == 0:
            new_data.append((0, 0, 0, 0))
        else:
            new_data.append(item)

    # 수정된 데이터를 이미지에 적용
    img_alpha.putdata(new_data)


    img_origin.putalpha(img_alpha.split()[3])
    # 수정된 이미지 저장 또는 표시
    img_origin.save(origin_name)


img_origin = any
origin = False
origin_name = ""
def handle_drop(event, label):
    global origin_name
    global origin
    global img_origin

    """Handle file drop event."""
    # 드롭된 파일 경로 가져오기
    file_path = event.data.strip('{}')  # 중괄호 제거
    file_name = os.path.basename(file_path)
    if("[alpha]" in file_name):
        label.config(text= "원본 이미지를 넣어주세요")
        return

    try:
        # 이미지 열기
        img_origin = Image.open(file_path)
        # 라벨에 파일 이름 표시
        label.config(text="로드된 파일: " + os.path.basename(file_path))  # 파일 이름만 표시
        origin_name = file_name
        origin = True
        if(alpha):
            SaveAlpha()
            root.destroy()
    except Exception as e:
        print("이미지를 열 수 없습니다:", e)


img_alpha = any
alpha = False
def handle_drop2(event, label):
    global img_alpha
    global alpha
    """Handle file drop event."""
    # 드롭된 파일 경로 가져오기
    file_path = event.data.strip('{}')
    file_name = os.path.basename(file_path)
    if("[alpha]" not in file_name):
        label.config(text= "alpha 이미지를 넣어주세요")
        return
    file_name = file_name.replace("[alpha]","")

    try:
        # 이미지 열기
        img_alpha = Image.open(file_path).convert("RGBA")
        label.config(text="로드된 파일: " + os.path.basename(file_name))
        alpha = True
        if(origin):
            SaveAlpha()
            root.destroy()
    except Exception as e:
        print("이미지를 열 수 없습니다:", e)




# Tkinter 윈도우 생성
root = TkinterDnD.Tk()
root.title("이미지 드래그 앤 드롭")
root.geometry("1000x500")  # 윈도우 크기 고정

# 드롭 가능한 영역 설정
frame1 = tk.Frame(root, width=250, height=500, bg="lightgray")
frame1.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)

frame2 = tk.Frame(root, width=250, height=500, bg="gray")
frame2.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True)

label1 = tk.Label(frame1, text="원본 이미지", bg="lightgray", font=("Arial", 20))
label1.place(relx=0.5, rely=0.5, anchor=tk.CENTER)

label2 = tk.Label(frame2, text="알파 이미지", bg="gray", fg="white", font=("Arial", 20))
label2.place(relx=0.5, rely=0.5, anchor=tk.CENTER)

# 드롭 이벤트 설정
frame1.drop_target_register(DND_FILES)
frame1.dnd_bind('<<Drop>>', lambda event: handle_drop(event, label1))

frame2.drop_target_register(DND_FILES)
frame2.dnd_bind('<<Drop>>', lambda event: handle_drop2(event, label2))

# 윈도우 실행
root.mainloop()
