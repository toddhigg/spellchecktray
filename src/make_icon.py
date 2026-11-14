"""Draw a dictionary icon and write a multi-size .ico."""
import struct
from pathlib import Path


def blend(dst, src):
    sr, sg, sb, sa = src
    if sa <= 0:
        return dst
    if sa >= 255:
        return src
    dr, dg, db, da = dst
    a = sa / 255
    inv = 1 - a
    return (
        int(sr * a + dr * inv),
        int(sg * a + dg * inv),
        int(sb * a + db * inv),
        int(sa + da * inv),
    )


def draw(size):
    px = [[(0, 0, 0, 0) for _ in range(size)] for _ in range(size)]

    def put(x, y, color):
        if 0 <= x < size and 0 <= y < size:
            px[y][x] = blend(px[y][x], color)

    def fill_round_rect(x0, y0, x1, y1, radius, color):
        r = max(1, radius)
        for y in range(y0, y1):
            for x in range(x0, x1):
                dx = 0
                dy = 0
                if x < x0 + r and y < y0 + r:
                    dx, dy = x0 + r - 1 - x, y0 + r - 1 - y
                elif x >= x1 - r and y < y0 + r:
                    dx, dy = x - (x1 - r), y0 + r - 1 - y
                elif x < x0 + r and y >= y1 - r:
                    dx, dy = x0 + r - 1 - x, y - (y1 - r)
                elif x >= x1 - r and y >= y1 - r:
                    dx, dy = x - (x1 - r), y - (y1 - r)
                if dx * dx + dy * dy <= r * r:
                    put(x, y, color)

    cover = (30, 74, 138, 255)
    page = (246, 239, 220, 255)
    spine = (18, 48, 96, 255)
    ink = (122, 96, 62, 255)
    ribbon = (194, 59, 46, 255)

    m = max(1, round(size * 0.1))
    top = max(1, round(size * 0.12))
    radius = max(1, round(size * 0.08))
    fill_round_rect(m, top, size - m, size - m, radius, cover)

    inset = max(2, round(size * 0.1))
    page_l = m + inset
    page_t = top + inset
    page_r = size - m - inset
    page_b = size - m - inset
    fill_round_rect(page_l, page_t, page_r, page_b, max(1, radius - 1), page)

    spine_w = max(2, round(size * 0.055))
    mid = size // 2
    for y in range(page_t, page_b):
        for x in range(mid - spine_w // 2, mid + (spine_w + 1) // 2):
            put(x, y, spine)

    line_h = max(1, size // 28)
    gap = max(3, round(size * 0.11))
    line_w = max(3, (page_r - mid - spine_w) // 2 - max(2, size // 16))
    y0 = page_t + max(2, round(size * 0.12))
    for side in (0, 1):
        if side == 0:
            x_start = page_l + max(2, size // 18)
        else:
            x_start = mid + spine_w // 2 + max(2, size // 18)
        for i in range(3):
            y = y0 + i * gap
            if y + line_h >= page_b - 1:
                break
            width = line_w if i < 2 else max(2, int(line_w * 0.62))
            for yy in range(y, y + line_h):
                for x in range(x_start, min(page_r - 1, x_start + width)):
                    put(x, yy, ink)

    mark_w = max(2, round(size * 0.09))
    mark_x = page_r - mark_w - max(2, size // 22)
    mark_top = top
    mark_bot = top + max(4, round(size * 0.34))
    for y in range(mark_top, mark_bot):
        for x in range(mark_x, mark_x + mark_w):
            put(x, y, ribbon)
    notch = max(2, mark_w // 2)
    for i in range(notch):
        for x in range(mark_x + i, mark_x + mark_w - i):
            put(x, mark_bot + i, ribbon)

    return px


def dib(pixels):
    size = len(pixels)
    xor_rows = bytearray()
    for y in range(size - 1, -1, -1):
        for x in range(size):
            r, g, b, a = pixels[y][x]
            xor_rows += bytes((b, g, r, a))
    mask_stride = ((size + 31) // 32) * 4
    mask = bytes(mask_stride * size)
    header = struct.pack(
        "<IiiHHIIiiII",
        40,
        size,
        size * 2,
        1,
        32,
        0,
        len(xor_rows) + len(mask),
        0,
        0,
        0,
        0,
    )
    return header + xor_rows + mask


def write_ico(path, sizes):
    blobs = [dib(draw(size)) for size in sizes]
    offset = 6 + 16 * len(blobs)
    entries = bytearray()
    body = bytearray()
    for size, blob in zip(sizes, blobs):
        dim = 0 if size >= 256 else size
        entries += struct.pack("<BBBBHHII", dim, dim, 0, 0, 1, 32, len(blob), offset)
        body += blob
        offset += len(blob)
    data = struct.pack("<HHH", 0, 1, len(blobs)) + entries + body
    Path(path).write_bytes(data)


if __name__ == "__main__":
    out = Path(__file__).with_name("Assets") / "Dictionary.ico"
    out.parent.mkdir(parents=True, exist_ok=True)
    write_ico(out, (16, 20, 24, 32, 48, 256))
    print(out, out.stat().st_size)
