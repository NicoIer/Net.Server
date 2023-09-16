# 把当前目录下的 代码文件夹 拷贝到 Unity 项目中
# ignored /bin /obj

target_dir="/Users/chenjun/Desktop/projects/Unity-Explore/Assets/Games/Moba/Network/NetCore/"

root="./"

# 先删除target_dir下的所有文件夹
for name in $(ls $target_dir)
do
  if test -d $target_dir/"$name"
  then
    rm -rf $target_dir/"$name"
  fi
done

for name in $(ls $root)
do
  if test -d $root/"$name"
  then
    # 名称不包含 bin  obj  Protobuf , Proto
    if [[ $name =~ "bin" ]] || [[ $name =~ "obj" ]] || [[ $name =~ "Protobuf" ]] || [[ $name =~ "Proto" ]]
    then
      continue
    fi
    cp -r $root/"$name" $target_dir
  fi
done