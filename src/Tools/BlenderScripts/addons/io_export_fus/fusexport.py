# Directly export the present blender scene to .fus
# Use this file when running blender from the command line (e.g. during build processes)
import bpy

def GetParents(obj):
    """Recursively search for the highest parent object"""
    if obj.parent == None:
        return obj
    elif obj.parent != None:
        GetParents(obj.parent)


def main():
    import sys       # to get command line args
    import argparse  # to parse options for us and print a nice help message

    import subprocess, os, sys, time

    # Add some necessary extras to blender's neutered python

    # Add anything from any existing full-fledged Python installation
    # because we need google.protobuf !!!
    paths = os.environ['Path']
    paths = paths.split(';')
    for path in paths:
        if path.find('Python')!=-1:
            if path.find('Blender')==-1:
                sPath=path.split('\\')
                if sPath[-2].find('Python')!=-1:
                    pypath = os.path.join(path,'Lib','site-packages')
                    sys.path.append(pypath)

    # Add the current path of THIS file
    this_path = os.path.dirname(os.path.realpath(__file__))
    sys.path.append(this_path)

    from BlenderVisitor import BlenderVisitor

    # get the args passed to blender after "--", all of which are ignored by
    # blender so scripts may receive their own arguments
    argv = sys.argv

    if "--" not in argv:
        argv = []  # as if no args are passed
    else:
        argv = argv[argv.index("--") + 1:]  # get all args after "--"

    # When --help or no args are given, print this help
    usage_text = (
        "Run blender in background mode to load a scene and export to fus:"
        "  blender --background MyScene.blend --python " + __file__ + " -- [options]"
    )

    parser = argparse.ArgumentParser(description=usage_text)

    # Example utility, add some text and renders or saves it (with options)
    # Possible types are: string, int, long, choice, float and complex.
    parser.add_argument(
        "-o", "--out", dest="fus_file", metavar='FILE',
        help="Destination path for the generated .fus file",
    )

    args = parser.parse_args(argv)  # In this example we won't use the args

    # if not argv:
    #     parser.print_help()
    #     return

    fus_file = ''
    if not args.fus_file:
        if not bpy.data.filepath:
            fus_file = 'untitled.fus'
        else:
            fus_file = os.path.splitext(bpy.data.filepath)[0] + '.fus'
    else:
        fus_file = args.fus_file

    
    # Run the exporter
    roots = set()
    for obj in bpy.data.objects:
        parent = GetParents(obj)
        roots.add(parent)
        try:
            roots.remove(None)
        except:
            pass

    #set blender to object mode and deselect everything (prevents problems)
    if bpy.context.mode != 'OBJECT' and bpy.ops.object.mode_set.poll():
        bpy.ops.object.mode_set(mode="OBJECT")
    bpy.ops.object.select_all(action='DESELECT')

    visitor = BlenderVisitor()

    visitor.DoApplyModifiers = True
    visitor.DoApplyScale = True
    visitor.DoRecalcOutside = True

    visitor.TraverseList(roots)
    # visitor.PrintFus()
    visitor.WriteFus(fus_file)

    print("Exported Blender scene to FUSEE file: " + fus_file)


if __name__ == "__main__":
    main()








