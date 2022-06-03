pub (in crate) fn f32_to_i32(float : f32) -> i32 {
    if f64::from(float) > f64::from(i32::MAX) || f64::from(float) < f64::from(i32::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        float.round() as i32
    }
}

pub (in crate) fn f32_to_u32(float : f32) -> u32 {
    if f64::from(float) > f64::from(u32::MAX) || f64::from(float) < f64::from(u32::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        float.round() as u32
    }
}

pub (in crate) fn f32_to_u16(float : f32) -> u16 {
    if f64::from(float) > f64::from(u16::MAX) || f64::from(float) < f64::from(u16::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        float.round() as u16
    }
}

pub (in crate) fn i32_to_f32(integer : i32) -> f32 {
    if f64::from(integer) > f64::from(f32::MAX) || f64::from(integer) < f64::from(f32::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        integer as f32
    }
}


pub (in crate) fn u32_to_f32(integer : u32) -> f32 {
    if f64::from(integer) > f64::from(f32::MAX) || f64::from(integer) < f64::from(f32::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        integer as f32
    }
}

pub (in crate) fn f32_to_u8(float : f32) -> u8 {
    if f64::from(float) > f64::from(u8::MAX) || f64::from(float) < f64::from(u8::MIN) {
        panic!("Could not convert due to input being out of bounds of the output type.");
    }
    else {
        float as u8
    }
}
