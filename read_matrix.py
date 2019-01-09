from __future__ import print_function

import csv
import os
import scipy.io
import shutil


matrix_dir = "/Users/vahid/Desktop/10xgenomics/filtered_peak_by_cell_matrix"


peaks = []
with open(os.path.join(matrix_dir, "peaks.bed"), "r") as f:
    for l in f:
        peaks.append(l)


clusters = {}
with open("/Users/vahid/Desktop/10xgenomics/clustering_analysis/clustering/kmedoids_2_clusters/clusters.csv", mode='r') as f:
    reader = csv.reader(f)
    clusters = {rows[0]: rows[1] for rows in reader}


cells_path = os.path.join(matrix_dir, "cells/")
if os.path.exists(cells_path):
    shutil.rmtree(cells_path)
os.makedirs(cells_path)


for k, v in clusters.iteritems():
    cluster_path = os.path.join(cells_path, v)
    if os.path.exists(cluster_path):
        shutil.rmtree(cluster_path)
    os.makedirs(cluster_path)


barcodeFiles = []
with open(os.path.join(matrix_dir, "barcodes.tsv")) as f:
    for l in f:
        barcode = l.strip()
        barcode_file = os.path.join(cells_path, clusters[barcode], "{}.txt".format(barcode))
        open(barcode_file, 'a').close()
        barcodeFiles.append(barcode_file)


mat = scipy.io.mmread(os.path.join(matrix_dir, "matrix.mtx"))
cx = scipy.sparse.coo_matrix(mat)


print(cx.get_shape())
print('\n\n\n\n>>>>>>>>>>>')
cx_array = cx.toarray()
print("rows:\t" + len(cx_array))
print("cols:\t" + len(cx_array[0]))

for col in len(cx_array[0]):
    print("processing cell " + col + " of " + cx_array[0], end="\r")
    with open(barcodeFiles[col - 1], "a+") as f:
        for row in len(cx_array):
            value = cx_array[row][col]
            if value > 0:
                f.write(peaks[row])
